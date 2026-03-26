using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AeroScape.Server.Core.Combat;
using AeroScape.Server.Core.Engine;
using AeroScape.Server.Core.Entities;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Services;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

/// <summary>
/// Handles Magic-on-Player packet (opcode 70).
/// Sets up PvP magic combat by resolving the spell and setting the combat target.
/// The actual casting is driven by the tick cycle's PlayerVsPlayerCombat service.
/// Ported from MagicOnPlayer / PacketManager logic.
/// </summary>
public class MagicOnPlayerMessageHandler : IMessageHandler<MagicOnPlayerMessage>
{
    private readonly ILogger<MagicOnPlayerMessageHandler> _logger;
    private readonly GameEngine _engine;
    private readonly MagicService _magic;
    private readonly IClientUiService _ui;

    public MagicOnPlayerMessageHandler(ILogger<MagicOnPlayerMessageHandler> logger, GameEngine engine, MagicService magic, IClientUiService ui)
    {
        _logger = logger;
        _engine = engine;
        _magic = magic;
        _ui = ui;
    }

    public Task HandleAsync(PlayerSession session, MagicOnPlayerMessage message, CancellationToken cancellationToken)
    {
        var player = session.Entity;
        if (player == null)
            return Task.CompletedTask;

        int targetId = message.PlayerId;

        if (targetId <= 0 || targetId >= GameEngine.MaxPlayers)
            return Task.CompletedTask;

        var target = _engine.Players[targetId];
        if (target == null || target.IsDead || !target.Online)
            return Task.CompletedTask;

        if (!CanAttack(player, target))
            return Task.CompletedTask;

        if (player.MagicDelay > 0)
            return Task.CompletedTask;

        bool casted = message.InterfaceId switch
        {
            388 when message.ButtonId == 3 => TryCastIceBarrage(player, target),
            192 or 193 when MagicSpellData.ButtonToSpellId.TryGetValue(message.ButtonId, out var spellId) => TryCastModernSpell(player, target, spellId),
            _ => false,
        };

        if (!casted)
        {
            _logger.LogDebug("[MagicOnPlayer] Unhandled spell interface={InterfaceId} button={ButtonId} player={Player}",
                message.InterfaceId, message.ButtonId, player.Username);
            return Task.CompletedTask;
        }

        // Skull in wilderness
        if (Player.IsWildernessArea(player.AbsX, player.AbsY) && player.SkulledDelay <= 0)
        {
            player.SkulledDelay = CombatConstants.SkullDuration;
            player.SkulledUpdateReq = true;
        }

        _logger.LogDebug("[MagicOnPlayer] Player {Player} magic-targeting player {Target}",
            player.Username, target.Username);

        return Task.CompletedTask;
    }

    private bool TryCastModernSpell(Player player, Player target, int spellId)
    {
        var spell = MagicSpellData.Spells[spellId];
        if (player.SkillLvl[CombatConstants.SkillMagic] < spell.LevelRequired || !_magic.TryConsumeCombatRunes(player, spell))
            return false;

        int damage = CombatFormulas.MagicDamage(spellId, player.SkillLvl[CombatConstants.SkillMagic], player.EquipmentBonus[CombatConstants.BonusMagicAttack]);
        damage = ApplyMagicProtection(target, damage);

        player.RequestFaceTo(target.PlayerId + 32768);
        player.RequestAnim(711, 0);
        player.RequestGfx(spell.CasterGfx, 100);
        target.RequestGfx(spell.VictimGfx, 177);
        target.AppendHit(damage, 0);
        target.RequestFaceTo(player.PlayerId + 32768);

        player.AddSkillXP(spell.GetXpForHit(damage), CombatConstants.SkillMagic);
        player.MagicDelay = CombatConstants.MagicCastDelay;
        player.CombatDelay = CombatConstants.MagicCastDelay;
        ResetNpcCombat(player);
        return true;
    }

    private bool TryCastIceBarrage(Player player, Player target)
    {
        if (player.SkillLvl[CombatConstants.SkillMagic] < 94)
        {
            _ui.SendMessage(player, "You need a magic level of 94 to cast this spell.");
            return false;
        }

        if (!HasRunes(player, (560, 4), (555, 6), (565, 2)))
        {
            _ui.SendMessage(player, "You don't have enough runes to cast this spell.");
            return false;
        }

        DeleteRune(player, 560, 4);
        DeleteRune(player, 555, 6);
        DeleteRune(player, 565, 2);

        int damage = ApplyMagicProtection(target, CombatFormulas.Random(30));
        player.RequestFaceTo(target.PlayerId + 32768);
        player.RequestAnim(1979, 0);
        target.RequestGfx(369, 0);
        target.AppendHit(damage, 0);
        target.RequestFaceTo(player.PlayerId + 32768);
        target.FreezeDelay = 10;
        player.AddSkillXP((52 + damage) * CombatConstants.MagicXpRate, CombatConstants.SkillMagic);
        player.MagicDelay = 5;
        player.CombatDelay += player.AttackDelay;
        ResetNpcCombat(player);
        return true;
    }

    private static int ApplyMagicProtection(Player target, int hitDamage)
    {
        if (target.PrayerIcon == 2)
        {
            if (target.Hitter > 0)
            {
                target.Hitter--;
                return 0;
            }

            target.Hitter = 2 + CombatFormulas.Random(4);
        }

        return hitDamage;
    }

    private static bool HasRunes(Player player, params (int ItemId, int Amount)[] runes)
    {
        foreach (var (itemId, amount) in runes)
        {
            int count = 0;
            for (int i = 0; i < player.Items.Length; i++)
            {
                if (player.Items[i] == itemId)
                    count += player.ItemsN[i] > 0 ? player.ItemsN[i] : 1;
            }

            if (count < amount)
                return false;
        }

        return true;
    }

    private static void DeleteRune(Player player, int itemId, int amount)
    {
        for (int i = 0; i < player.Items.Length && amount > 0; i++)
        {
            if (player.Items[i] != itemId)
                continue;

            int stack = player.ItemsN[i] > 0 ? player.ItemsN[i] : 1;
            int remove = stack > amount ? amount : stack;
            stack -= remove;
            amount -= remove;
            if (stack > 0)
            {
                player.ItemsN[i] = stack;
            }
            else
            {
                player.Items[i] = -1;
                player.ItemsN[i] = 0;
            }
        }
    }

    private static void ResetNpcCombat(Player player)
    {
        player.AttackingNPC = false;
        player.AttackNPC = 0;
        player.AttackingPlayer = false;
        player.AttackPlayer = 0;
    }

    private static bool CanAttack(Player player, Player target)
    {
        if (player.AbsY - 3520 < 1 && !IsAtDuel(player) && !IsAtClanField(player) && !IsAtCastleWars(player))
            return false;
        if (IsAtDuel(player) && player.DuelPartner != target.PlayerId)
            return false;
        if (IsAtDuel(player) && !player.DuelCan)
            return false;
        if (IsAtClanField(player) && player.ClanChat == target.ClanChat)
            return false;
        if (IsAtClanField(player) && player.OverTimer > 0)
            return false;
        return !IsAtJail(player) && !IsAtClanLobby(player);
    }

    private static bool IsAtDuel(Player player) => player.AbsX >= 3362 && player.AbsX <= 3391 && player.AbsY >= 3228 && player.AbsY <= 3241;
    private static bool IsAtClanField(Player player) => player.AbsX >= 3268 && player.AbsX <= 3385 && player.AbsY >= 3520 && player.AbsY <= 3607;
    private static bool IsAtCastleWars(Player player) => player.AbsX >= 2368 && player.AbsX <= 2428 && player.AbsY >= 3072 && player.AbsY <= 3132;
    private static bool IsAtJail(Player player) => player.AbsX >= 2602 && player.AbsX <= 2610 && player.AbsY >= 3102 && player.AbsY <= 3110;
    private static bool IsAtClanLobby(Player player) => player.AbsX >= 3270 && player.AbsX <= 3382 && player.AbsY >= 3672 && player.AbsY <= 3701;
}
