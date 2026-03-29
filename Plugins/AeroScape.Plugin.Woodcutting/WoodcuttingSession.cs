using AeroScape.Server.API.Models;
using static AeroScape.Plugin.Woodcutting.TreeDefinitions;
using static AeroScape.Plugin.Woodcutting.AxeDefinitions;

namespace AeroScape.Plugin.Woodcutting;

/// <summary>
/// Represents an active woodcutting session for a player.
/// </summary>
internal class WoodcuttingSession
{
    public required PlayerInfo Player { get; init; }
    public required TreeDefinition Tree { get; init; }
    public required AxeDefinition Axe { get; init; }
    public required DateTime StartTime { get; init; }
    public int LogsChopped { get; set; }
    public int TicksUntilNextLog { get; set; } = 4; // Default 4 ticks per log
    public int AnimationCounter { get; set; } = 0;
    public bool IsChopping { get; set; } = true;
}