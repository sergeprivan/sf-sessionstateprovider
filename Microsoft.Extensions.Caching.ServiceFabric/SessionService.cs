using Microsoft.Extensions.Caching.ServiceFabric.UserSession.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Fabric;
using Microsoft.ServiceFabric.Services.Remoting.Client;

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

        public async Task AddSessionItem(string userSessionId, UserSessionItem userSessionItem)
        {
            try
            {
                //
                // In fact we are doing here a request to get a needed service based on partion key generated from userSessionId and
                // on next step we will need to get an item with userSessionId, this looks as not nessesary query but we have a different service for the differnt data ( sharding )
                //

                var serSessionService = GetSessionServiceByUserSessionId(userSessionId);

                await serSessionService?.SetSessionItem(userSessionItem, CancellationToken.None);
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.Message($"Method {nameof(SessionService)}->AddSessionItem<T>() failed with error: {ex.ToString()} at: {DateTime.UtcNow}");
            }
        }
                             

        public async Task<UserSessionItem> GetSessionItem(string userSessionId, string key)
        {
            try
            {
                //
                // In fact we are doing here a request to get a needed service based on partion key generated from userSessionId and
                // on next step we will need to get an item with userSessionId, this looks as not nessesary query but we have a different service for the differnt data ( sharding )
                //
                var serSessionService = GetSessionServiceByUserSessionId(userSessionId);

                var sessionItemValue = await serSessionService?.GetSessionItem(new UserSessionItemId(key), CancellationToken.None);

                return sessionItemValue;
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
                //
                // In fact we are doing here a request to get a needed service based on partion key generated from userSessionId and
                // on next step we will need to get an item with userSessionId, this looks as not nessesary query but we have a different service for the differnt data ( sharding )
                //

                var userSessionService = GetSessionServiceByUserSessionId(userSessionId);

                await userSessionService?.RemoveSessionItem(new UserSessionItemId(key), CancellationToken.None);
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.Message($"Method {nameof(SessionService)}->RemoveSessionItem<T>() failed with error: {ex.ToString()} at: {DateTime.UtcNow}");
            }
        }

        public async Task RemoveSessionItem(string userSessionId, UserSessionItem userSessionItem)
        {
            try
            {
                //
                // In fact we are doing here a request to get a needed service based on partion key generated from userSessionId and
                // on next step we will need to get an item with userSessionId, this looks as not nessesary query but we have a different service for the differnt data ( sharding )
                //

                var userSessionService = GetSessionServiceByUserSessionId(userSessionId);

                await userSessionService?.RemoveSessionItem(userSessionItem.Id, CancellationToken.None);
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.Message($"Method {nameof(SessionService)}->RemoveSessionItem<T>() failed with error: {ex.ToString()} at: {DateTime.UtcNow}");
            }

        }

        public IUserSessionService GetSessionServiceByUserSessionId(string userSessionId)
        {
            try
            {
                var userSessionService = ServiceProxy.Create<IUserSessionService>(_keyServiceUri, new UserSessionItemId(userSessionId).GetPartitionKey());
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
