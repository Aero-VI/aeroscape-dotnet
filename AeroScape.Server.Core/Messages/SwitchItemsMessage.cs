namespace AeroScape.Server.Core.Messages;

public record SwitchItemsMessage(int ToSlot, int FromSlot, int InterfaceId);
