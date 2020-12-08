using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Caching.ServiceFabric.UserSession.Interfaces
{
    public interface IUserSessionService : IService
    {
        Task<SessionKeyItem> GetSessionItem(SessionKeyItemId sessionKeyItemId, CancellationToken cancellationToken);
        Task SetSessionItem(SessionKeyItem sessionKeyItem, CancellationToken cancellationToken);
        Task RemoveSessionItem(SessionKeyItemId sessionKeyItemId, CancellationToken cancellationToken);
    }
}
