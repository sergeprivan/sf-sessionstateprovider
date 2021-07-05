using Microsoft.Extensions.Caching.ServiceFabric.UserSession.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Caching.ServiceFabric
{
    public interface ISessionService
    {
        /* Task AddSessionItem(string userSessionId, string key, byte[] value);
        Task<byte[]> GetSessionItem(string userSessionId, string key);
        Task RemoveSessionItem(string userSessionId, string key); */

        Task AddSessionItem(string userSessionId, UserSessionItem userSessionItem);
        Task RemoveSessionItem(string userSessionId, UserSessionItem userSessionItem);
        Task<UserSessionItem> GetSessionItem(string userSessionId, string key);

    }
}
