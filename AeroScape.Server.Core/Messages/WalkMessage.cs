namespace AeroScape.Server.Core.Messages;

public sealed record WalkMessage(
    int PacketId,
    int FirstX,
    int FirstY,
    bool IsRunning,
    int[] PathX,
    int[] PathY);
