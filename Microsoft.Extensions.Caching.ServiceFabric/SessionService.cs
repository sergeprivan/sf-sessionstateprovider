using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.Extensions.Caching.ServiceFabric.UserSession.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Caching.ServiceFabric
{
    public class SessionService : ISessionService
    {
        public SessionService()
        {
            UserSessionUriBuilder = new ServiceUriBuilder("UserSessionActorService");
        }

        public async Task AddSessionItem(string userSessionId, string key, byte[] value)
        {
            try
            {
                var sessionActor = GetSessionActor(userSessionId);
                await sessionActor?.SetSessionItem(key, value, CancellationToken.None);
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.Message($"Method {nameof(SessionService)}->AddSessionItem<T>() failed with error: {ex.ToString()} at: {DateTime.UtcNow}");
            }
        }

        public async Task<byte[]> GetSessionItem(string userSessionId, string key)
        {
            try
            {
                var sessionActor = GetSessionActor(userSessionId);
                var sessionItemValue = await sessionActor?.GetSessionItem(key, CancellationToken.None);
                if (sessionItemValue == null)
                {
                    return null;
                }
                return sessionItemValue;
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.Message($"Method {nameof(SessionService)}->GetSessionItem<T>() failed with error: {ex.ToString()} at: {DateTime.UtcNow}");
            }
            return null;
        }

        public IUserSession GetSessionActor(string userSessionId)
        {

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
