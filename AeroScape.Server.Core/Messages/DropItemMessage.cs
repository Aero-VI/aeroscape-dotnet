namespace AeroScape.Server.Core.Messages;

public record DropItemMessage(int ItemId, int Slot, int InterfaceId);
