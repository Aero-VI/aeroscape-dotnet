using Microsoft.Extensions.Logging;
using AeroScape.Server.API;
using AeroScape.Server.API.Events;
using AeroScape.Server.API.Models;
using static AeroScape.Plugin.Woodcutting.TreeDefinitions;
using static AeroScape.Plugin.Woodcutting.AxeDefinitions;

namespace AeroScape.Plugin.Woodcutting;

/// <summary>
/// Main woodcutting plugin implementation.
/// </summary>
public class WoodcuttingPlugin : IPlugin
{
    private IPluginAPI _api = null!;
    private readonly Dictionary<string, WoodcuttingSession> _sessions = new();
    
    public PluginInfo Info => new()
    {
        Name = "Woodcutting",
        Version = "1.0.0",
        Author = "AeroScape Team",
        Description = "Woodcutting skill implementation"
    };

    public void OnLoad(IPluginAPI api)
    {
        _api = api;
        _api.Logger.LogInformation("Woodcutting plugin loading...");
    }

    public void OnEnable()
    {
        // Register event handlers
        _api.Events.RegisterHandler<PlayerInteractObjectEventArgs>(OnPlayerInteractObject, EventPriority.Normal);
        _api.Events.RegisterHandler<PlayerTickEventArgs>(OnPlayerTick, EventPriority.Normal);
        _api.Events.RegisterHandler<PlayerLogoutEventArgs>(OnPlayerLogout, EventPriority.Normal);
        
        _api.Logger.LogInformation("Woodcutting plugin enabled!");
    }

    public void OnDisable()
    {
        // Clean up all sessions
        _sessions.Clear();
        _api.Logger.LogInformation("Woodcutting plugin disabled.");
    }

    public void OnUnload()
    {
        // Final cleanup
        _api.Logger.LogInformation("Woodcutting plugin unloaded.");
    }

    private void OnPlayerInteractObject(object? sender, PlayerInteractObjectEventArgs e)
    {
        // Only handle first option (left-click)
        if (e.Option != 1)
            return;
            
        // Check if object is a tree
        var tree = FindTree(e.ObjectId);
        if (tree == null) 
            return;
        
        // Check player level
        var wcLevel = _api.Skills.GetLevel(e.Player, SkillIds.Woodcutting);
        if (wcLevel < tree.LevelRequired)
        {
            _api.Players.SendMessage(e.Player, $"You need level {tree.LevelRequired} Woodcutting to chop this tree.");
            e.IsCancelled = true;
            return;
        }
        
        // Find the best axe the player can use
        var axe = FindBestAxe(e.Player, wcLevel);
        if (axe == null)
        {
            _api.Players.SendMessage(e.Player, "You don't have an axe which you have the woodcutting level to use.");
            e.IsCancelled = true;
            return;
        }
        
        // Check inventory space
        if (_api.Inventory.GetFreeSlots(e.Player) < 1)
        {
            _api.Players.SendMessage(e.Player, "Not enough inventory space!");
            e.IsCancelled = true;
            return;
        }
        
        // Start woodcutting session
        _sessions[e.Player.Username] = new WoodcuttingSession
        {
            Player = e.Player,
            Tree = tree,
            Axe = axe,
            StartTime = DateTime.UtcNow,
            LogsChopped = 0,
            TicksUntilNextLog = 4
        };
        
        _api.Players.PlayAnimation(e.Player, axe.Animation);
        _api.Players.SendMessage(e.Player, "You swing your axe at the tree.");
        
        e.IsCancelled = true; // Prevent default handling
        
        _api.Logger.LogDebug("Player {Player} started chopping {Tree} with {Axe}", 
            e.Player.Username, tree.Name, axe.Name);
    }

    private void OnPlayerTick(object? sender, PlayerTickEventArgs e)
    {
        if (!_sessions.TryGetValue(e.Player.Username, out var session))
            return;
            
        // Only process if player is still at the tree
        if (!session.IsChopping)
        {
            _sessions.Remove(e.Player.Username);
            return;
        }
        
        // Animation every 2 ticks
        session.AnimationCounter++;
        if (session.AnimationCounter >= 2)
        {
            _api.Players.PlayAnimation(e.Player, session.Axe.Animation);
            session.AnimationCounter = 0;
        }
        
        // Check if it's time to get a log
        session.TicksUntilNextLog--;
        if (session.TicksUntilNextLog <= 0)
        {
            ProcessLogChop(e.Player, session);
        }
    }

    private void ProcessLogChop(PlayerInfo player, WoodcuttingSession session)
    {
        // Check inventory space again
        if (_api.Inventory.GetFreeSlots(player) < 1)
        {
            _api.Players.SendMessage(player, "Not enough inventory space to cut any more logs!");
            _sessions.Remove(player.Username);
            return;
        }
        
        // Grant the log
        _api.Inventory.AddItem(player, session.Tree.LogItemId);
        session.LogsChopped++;
        
        // Calculate and grant XP: (BaseXp * wcLevel) / 3
        var wcLevel = _api.Skills.GetLevel(player, SkillIds.Woodcutting);
        var xp = (session.Tree.BaseXp * wcLevel) / 3.0;
        _api.Skills.AddExperience(player, SkillIds.Woodcutting, xp);
        
        _api.Players.SendMessage(player, "You get some logs.");
        
        // Check if tree is depleted
        if (session.LogsChopped >= session.Tree.MaxLogs)
        {
            _api.Players.SendMessage(player, "The tree has been chopped down.");
            _sessions.Remove(player.Username);
            
            // TODO: Could spawn a tree stump object and schedule respawn
            return;
        }
        
        // Continue chopping
        session.TicksUntilNextLog = 4;
    }

    private void OnPlayerLogout(object? sender, PlayerLogoutEventArgs e)
    {
        // Clean up session when player logs out
        _sessions.Remove(e.Player.Username);
    }

    private AxeDefinition? FindBestAxe(PlayerInfo player, int wcLevel)
    {
        // Axes are ordered best → worst; return first usable one
        foreach (var axe in Axes)
        {
            if (wcLevel >= axe.LevelRequired && _api.Inventory.HasItemOrEquipped(player, axe.ItemId))
                return axe;
        }
        return null;
    }
}