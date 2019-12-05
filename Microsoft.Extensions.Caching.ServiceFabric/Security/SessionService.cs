using Microsoft.AspNetCore.Http;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.Extensions.Caching.ServiceFabric.UserSession.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Caching.ServiceFabric.Security {
    public class SessionService: ISessionService {
        public SessionService() {
            UserSessionUriBuilder = new ServiceUriBuilder("UserSessionActorService");
        }

        public async Task AddSessionItem<T>(string userSessionId, string key, T value) {
            try {
                var sessionActor = GetSessionActor(userSessionId);
                await sessionActor?.SetSessionItem(key, value.ToString(), CancellationToken.None);
            }
            catch (Exception ex) {
                ServiceEventSource.Current.Message($"Method {nameof(SessionService)}->AddSessionItem<T>() failed with error: {ex.ToString()} at: {DateTime.UtcNow}");
            }
        }

        public async Task<T> GetSessionItem<T>(string userSessionId, string key) {
            try {
                var sessionActor = GetSessionActor(userSessionId);
                var sessionItemValue = await sessionActor?.GetSessionItem(key, CancellationToken.None);
                if (string.IsNullOrEmpty(sessionItemValue)) {
                    return default(T);
                }
                return (T)Convert.ChangeType(sessionItemValue, typeof(T));
            }
            catch (Exception ex) {
                ServiceEventSource.Current.Message($"Method {nameof(SessionService)}->GetSessionItem<T>() failed with error: {ex.ToString()} at: {DateTime.UtcNow}");
            }
            return default(T);
        }

        public IUserSession GetSessionActor(string userSessionId) {

            if (string.IsNullOrEmpty(userSessionId))
            {
                return null;
            }
            var actorId = new ActorId(userSessionId);
            return ActorProxy.Create<IUserSession>(actorId, UserSessionUriBuilder.ToUri());
        }


        ServiceUriBuilder UserSessionUriBuilder { get; }
    }
}
