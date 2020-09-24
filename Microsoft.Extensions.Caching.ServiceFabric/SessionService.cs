using Microsoft.Extensions.Caching.ServiceFabric.UserSession.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Fabric;
using System.Fabric.Query;
using System.Collections.Generic;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Microsoft.Extensions.Caching.ServiceFabric
{
    public class SessionService : ISessionService
    {
        // TODO share FabricClient for better performance https://stackoverflow.com/questions/37774965/how-to-enumerate-all-partitions-and-aggregate-results
        static FabricClient _fabricClient = new FabricClient();
        static Uri _keyServiceUri = new ServiceUriBuilder("Microsoft.Extensions.Caching.ServiceFabric.SessionKeys.ProtectionKeys").ToUri();


        public SessionService()
        {
        }

        public async Task AddSessionItem(string userSessionId, string key, byte[] value)
        {
            try
            {
                var sessionActor = GetSessionActor(userSessionId);

                var sessionKeyItem = new SessionKeyItem(key, value);

                await sessionActor?.SetSessionItem(sessionKeyItem, CancellationToken.None);
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
                
                var sessionItemValue = await sessionActor?.GetSessionItem(new SessionKeyItemId(userSessionId), CancellationToken.None);
                
                return sessionItemValue.Value;
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.Message($"Method {nameof(SessionService)}->GetSessionItem<T>() failed with error: {ex.ToString()} at: {DateTime.UtcNow}");
            }
            return null;
        }

        public async Task RemoveSessionItem(string userSessionId, string key)
        {
            try
            {
                var sessionActor = GetSessionActor(userSessionId);
                //await sessionActor?.RemoveSessionItem(key, CancellationToken.None);
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.Message($"Method {nameof(SessionService)}->RemoveSessionItem<T>() failed with error: {ex.ToString()} at: {DateTime.UtcNow}");
            }
        }

        public IUserSessionService GetSessionActor(string userSessionId)
        {
            try
            {
                var userSessionService = ServiceProxy.Create<IUserSessionService>(_keyServiceUri, new SessionKeyItemId(userSessionId).GetPartitionKey());
                //var sessionItem = userSessionService.GetSessionItem(new SessionKeyItemId(), CancellationToken.None);
                return userSessionService;
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.Message($"{nameof(ServiceFabricDataProtectionRepository)}->StoreElement() failed to save Data Protection Keys with error: {ex.ToString()}");
            }

            return null;
        }             

    }
}
