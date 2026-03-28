namespace AeroScape.Server.Core.Services;

/// <summary>
/// Minimal dummy implementation - magic disabled
/// </summary>
public class MagicService
{
    public bool TryCastModernAction(object player, int action) => false;
    public bool TryCastAncientAction(object player, int action) => false;
}