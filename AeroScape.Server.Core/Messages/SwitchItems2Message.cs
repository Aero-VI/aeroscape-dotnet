namespace AeroScape.Server.Core.Messages;

/// <summary>
/// Extended item switch / bank tab drag message.
/// Carries both interface and tab context for bank reorganisation.
/// </summary>
public record SwitchItems2Message(int ToInterface, int FromInterface, int FromSlot, int ToSlot, int InterfaceId, int TabId);
