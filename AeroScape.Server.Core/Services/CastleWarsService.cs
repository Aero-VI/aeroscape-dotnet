using AeroScape.Server.Core.Entities;

namespace AeroScape.Server.Core.Services;

public sealed class CastleWarsService
{
    private readonly IClientUiService _ui;

    public CastleWarsService(IClientUiService ui)
    {
        _ui = ui;
    }

    public void IncreaseFLZam(Player player, int increasedBy)
    {
        player.ZamFL += increasedBy;
        _ui.UpdateCastleWarsCounters(player);
    }

    public void IncreaseFLSara(Player player, int increasedBy)
    {
        player.SaraFL += increasedBy;
        _ui.UpdateCastleWarsCounters(player);
    }
}
