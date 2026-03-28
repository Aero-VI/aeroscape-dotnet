using AeroScape.Server.Core.Entities;

namespace AeroScape.Server.Core.Services;

/// <summary>
/// Minimal dummy implementation - NPC interactions disabled
/// </summary>
public class NPCInteractionService
{
    public void RegisterNpc(NPC npc) { }
    public void HandleNPCOption1(Player player, NPC npc) { }
    public void HandleNPCOption2(Player player, NPC npc) { }
    public void HandleNPCOption3(Player player, NPC npc) { }
}