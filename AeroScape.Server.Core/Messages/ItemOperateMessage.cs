namespace AeroScape.Server.Core.Messages;

public record ItemOperateMessage(int ItemId, int SlotId, int InterfaceId);