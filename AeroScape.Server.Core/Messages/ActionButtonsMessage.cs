namespace AeroScape.Server.Core.Messages;

public record ActionButtonsMessage(int PacketOpcode, int InterfaceId, int ButtonId, int ItemId, int SlotId);
