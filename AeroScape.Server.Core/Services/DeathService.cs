using AeroScape.Server.Core.Entities;

namespace AeroScape.Server.Core.Services;

/// <summary>
/// Minimal dummy implementation - death disabled
/// </summary>
public class DeathService
{
    public void ProcessPlayerDeath(Player p) { }
    public void RestoreAfterDeath(Player p) { }
    public void ProcessNpcDeath(NPC n) { }
}