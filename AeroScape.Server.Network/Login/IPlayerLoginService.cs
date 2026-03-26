using AeroScape.Server.Core.Entities;

namespace AeroScape.Server.Network.Login;

/// <summary>
/// Abstraction for player load/create during login.
/// Implemented in the App layer where EF Core context is available.
/// </summary>
public interface IPlayerLoginService
{
    /// <summary>
    /// Try to load or create a player. Returns a tuple of (Player, returnCode).
    /// Return codes: 2 = success, 3 = wrong password, 4 = banned, 5 = already online.
    /// </summary>
    Task<(Player player, int returnCode)> LoadOrCreatePlayerAsync(string username, string password);
}
