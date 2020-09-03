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
        //Task<long> GetKeyCountAsync(CancellationToken cancellationToken);
        //Task AddKey(SessionKeyItem keyItem);
        //Task<IEnumerable<SessionKeyItem>> GetKeys(CancellationToken cancellationToken);

        Task<byte[]> GetSessionItem(string key, CancellationToken cancellationToken);
        Task SetSessionItem(string key, byte[] value, CancellationToken cancellationToken);
        Task RemoveSessionItem(string key, CancellationToken cancellationToken);
    }
}
