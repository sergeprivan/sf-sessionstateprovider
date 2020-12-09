using Microsoft.ServiceFabric.Services.Remoting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Caching.ServiceFabric.UserSession.Interfaces
{
    /// <summary>
    /// Main interface to store and manipulate with session data based on user session data
    /// </summary>
    public interface IUserSessionService : IService
    {
        Task<SessionKeyItem> GetSessionItem(SessionKeyItemId sessionKeyItemId, CancellationToken cancellationToken);
        Task SetSessionItem(SessionKeyItem sessionKeyItem, CancellationToken cancellationToken);
        Task RemoveSessionItem(SessionKeyItemId sessionKeyItemId, CancellationToken cancellationToken);
    }
}
