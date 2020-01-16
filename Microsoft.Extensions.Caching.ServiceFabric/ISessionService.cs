using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Caching.ServiceFabric
{
    public interface ISessionService
    {
        Task AddSessionItem(string userSessionId, string key, byte[] value);
        Task<byte[]> GetSessionItem(string userSessionId, string key);
        Task RemoveSessionItem(string userSessionId, string key);
    }
}
