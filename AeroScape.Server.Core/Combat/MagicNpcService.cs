using AeroScape.Server.Core.Entities;

namespace AeroScape.Server.Core.Combat;

/// <summary>
/// Legacy MagicNPC.java helpers used by the PvE combat loop.
/// Handles autocast staff checks and rune consumption, including elemental staff rune removal.
/// </summary>
public static class MagicNpcService
{
    public static bool HasAutocastStaff(Player player)
        => MagicSpellData.IsAutocastStaff(player.Equipment[CombatConstants.SlotWeapon]);

    public static bool TryConsumeRunes(Player player, SpellDefinition spell)
    {
        foreach (var requirement in spell.RuneRequirements)
        {
            if (IsProvidedByStaff(player, requirement.RuneId))
                continue;

            if (CountItem(player, requirement.RuneId) < requirement.Amount)
                return false;
        }

        foreach (var requirement in spell.RuneRequirements)
        {
            if (IsProvidedByStaff(player, requirement.RuneId))
                continue;

            DeleteItem(player, requirement.RuneId, requirement.Amount);
        }

        return true;
    }

    private static bool IsProvidedByStaff(Player player, int runeId)
    {
        var weaponId = player.Equipment[CombatConstants.SlotWeapon];
        return MagicSpellData.StaffRuneMap.TryGetValue(weaponId, out var providedRune)
               && providedRune == runeId;
    }

    private static int CountItem(Player player, int itemId)
    {
        var count = 0;
        for (var i = 0; i < player.Items.Length; i++)
        {
            if (player.Items[i] != itemId)
                continue;

            count += player.ItemsN[i] > 0 ? player.ItemsN[i] : 1;
        }

        return count;
    }

    private static void DeleteItem(Player player, int itemId, int amount)
    {
        for (var i = 0; i < player.Items.Length && amount > 0; i++)
        {
            if (player.Items[i] != itemId)
                continue;

            var stack = player.ItemsN[i] > 0 ? player.ItemsN[i] : 1;
            if (stack > amount)
            {
                player.ItemsN[i] = stack - amount;
                return;
            }

            amount -= stack;
            player.Items[i] = -1;
            player.ItemsN[i] = 0;
        }
    }
}
