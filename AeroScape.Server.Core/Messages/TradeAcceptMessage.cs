namespace AeroScape.Server.Core.Messages;

/// <summary>Player accepts a trade in the second trade screen (opcode 253).</summary>
public record TradeAcceptMessage(int PartnerId);
