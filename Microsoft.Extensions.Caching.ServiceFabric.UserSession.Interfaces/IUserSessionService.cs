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
        Task<UserSessionItem> GetSessionItem(UserSessionItemId sessionKeyItemId, CancellationToken cancellationToken);
        Task SetSessionItem(UserSessionItem sessionKeyItem, CancellationToken cancellationToken);
        Task RemoveSessionItem(UserSessionItemId sessionKeyItemId, CancellationToken cancellationToken);
    }
}
