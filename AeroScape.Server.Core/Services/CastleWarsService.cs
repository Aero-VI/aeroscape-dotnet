using AeroScape.Server.Core.Entities;

namespace AeroScape.Server.Core.Services;

public sealed class CastleWarsService
{
    public void IncreaseFLZam(Player player, int increasedBy)
        => player.ZamFL += increasedBy;

    public void IncreaseFLSara(Player player, int increasedBy)
        => player.SaraFL += increasedBy;
}
