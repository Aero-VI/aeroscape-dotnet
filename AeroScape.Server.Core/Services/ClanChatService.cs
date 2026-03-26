using System;
using System.Collections.Concurrent;
using AeroScape.Server.Core.Engine;
using AeroScape.Server.Core.Entities;

namespace AeroScape.Server.Core.Services;

public sealed class ClanChatService
{
    private readonly GameEngine _engine;
    private readonly IClientUiService _ui;
    private readonly ConcurrentDictionary<string, ClanChannel> _channels = new(StringComparer.OrdinalIgnoreCase);

    public ClanChatService(GameEngine engine, IClientUiService ui)
    {
        _engine = engine;
        _ui = ui;
    }

    public void CreateOrRenameChat(Player owner, string clanName)
    {
        var channel = _channels.AddOrUpdate(
            owner.Username,
            _ => new ClanChannel(owner.Username, clanName),
            (_, existing) =>
            {
                existing.ClanName = clanName;
                return existing;
            });

        owner.ClanName = channel.ClanName;
        _ui.SendMessage(owner, $"You changed the name of your clan to: {channel.ClanName}");
    }

    public bool JoinChat(Player player, string ownerName)
    {
        LeaveChat(player);

        if (!_channels.TryGetValue(ownerName, out var channel))
        {
            int ownerId = _engine.GetIdFromName(ownerName);
            var owner = ownerId > 0 ? _engine.Players[ownerId] : null;
            if (owner is null)
                return false;

            channel = _channels.GetOrAdd(owner.Username, _ => new ClanChannel(owner.Username, string.IsNullOrWhiteSpace(owner.ClanName) ? owner.Username : owner.ClanName));
        }

        if (string.IsNullOrWhiteSpace(channel.ClanName) || channel.JoinRequirement > GetRank(channel, player.Username))
            return false;

        channel.Members[player.Username] = new ClanMember(player.Username);
        player.ClanName = channel.ClanName;
        player.ClanChannel = 1;
        _ui.SendMessage(player, $"You are now talking in: {channel.ClanName}");
        return true;
    }

    public void LeaveChat(Player player)
    {
        foreach (var channel in _channels.Values)
            channel.Members.TryRemove(player.Username, out _);

        player.ClanChannel = 0;
        _ui.ResetClanChatList(player);
    }

    public bool SendMessage(Player player, string message)
    {
        foreach (var channel in _channels.Values)
        {
            if (!channel.Members.ContainsKey(player.Username))
                continue;

            if (channel.TalkRequirement > GetRank(channel, player.Username))
                return false;

            channel.LastMessage = (player.Username, message);
            foreach (var member in channel.Members.Values)
            {
                int id = _engine.GetIdFromName(member.Name);
                var target = id > 0 ? _engine.Players[id] : null;
                if (target is not null)
                    _ui.SendClanChat(target, player, channel.ClanName, message);
            }
            return true;
        }

        return false;
    }

    public void RankPlayer(Player owner, string name, int rank)
    {
        if (!_channels.TryGetValue(owner.Username, out var channel))
            return;

        channel.Ranks[name] = rank;
    }

    public bool Kick(Player owner, string name)
    {
        if (!_channels.TryGetValue(owner.Username, out var channel))
            return false;

        if (!channel.Members.TryRemove(name, out _))
            return false;

        int id = _engine.GetIdFromName(name);
        var target = id > 0 ? _engine.Players[id] : null;
        if (target is not null)
        {
            target.ClanChannel = 0;
            _ui.SendMessage(target, "You've been kick from the chat.");
            _ui.ResetClanChatList(target);
        }

        return true;
    }

    public void SetRequirement(Player owner, int requirementType, int value)
    {
        if (!_channels.TryGetValue(owner.Username, out var channel))
            return;

        switch (requirementType)
        {
            case 1:
                channel.JoinRequirement = value;
                break;
            case 2:
                channel.TalkRequirement = value;
                break;
            case 3:
                channel.KickRequirement = value;
                break;
        }
    }

    public void SetLootShare(Player player, bool enabled)
    {
        var channel = FindPlayersChannel(player);
        if (channel is not null)
            channel.LootShareOn = enabled;
    }

    public bool LootShareOn(Player player)
        => FindPlayersChannel(player)?.LootShareOn == true;

    private ClanChannel? FindPlayersChannel(Player player)
    {
        foreach (var channel in _channels.Values)
            if (channel.Members.ContainsKey(player.Username))
                return channel;

        return null;
    }

    private static int GetRank(ClanChannel channel, string name)
        => channel.Owner.Equals(name, StringComparison.OrdinalIgnoreCase)
            ? 7
            : channel.Ranks.GetValueOrDefault(name, 0);

    private sealed class ClanChannel(string owner, string clanName)
    {
        public string Owner { get; } = owner;
        public string ClanName { get; set; } = clanName;
        public int JoinRequirement { get; set; }
        public int TalkRequirement { get; set; }
        public int KickRequirement { get; set; } = 7;
        public bool LootShareOn { get; set; }
        public ConcurrentDictionary<string, int> Ranks { get; } = new(StringComparer.OrdinalIgnoreCase);
        public ConcurrentDictionary<string, ClanMember> Members { get; } = new(StringComparer.OrdinalIgnoreCase);
        public (string PlayerName, string Message) LastMessage { get; set; }
    }

    private sealed class ClanMember(string name)
    {
        public string Name { get; } = name;
        public int Chance { get; set; }
    }
}
