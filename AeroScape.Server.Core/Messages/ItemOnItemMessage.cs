namespace AeroScape.Server.Core.Messages;

public record ItemOnItemMessage(int UsedWithId, int ItemUsedId, int UsedWithSlot, int ItemUsedSlot, int Interface1, int Interface2);
