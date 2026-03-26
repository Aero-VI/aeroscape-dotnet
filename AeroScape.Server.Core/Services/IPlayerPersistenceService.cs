using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Entities;

namespace AeroScape.Server.Core.Services;

public interface IPlayerPersistenceService
{
    Task SavePlayerAsync(Player player, CancellationToken cancellationToken = default);
}
