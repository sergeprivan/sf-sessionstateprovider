using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Data;
using Microsoft.Extensions.Caching.ServiceFabric.UserSession.Interfaces;
using System.Fabric;

namespace Microsoft.Extensions.Caching.ServiceFabric.UserSession
{
    /// <remarks>
    /// This class represents and user session service for dealing with session data
    /// </remarks>
    internal class UserSessionService : StatefulService, IUserSessionService
    {
        private const string SessionKeyDictionaryName = "Microsoft.Extensions.Caching.ServiceFabric.UserSession.UserSessionService.Data";

        public async Task<UserSessionItem> GetSessionItem(UserSessionItemId sessionKeyItemId, CancellationToken cancellationToken)
        {
            var sessionKeyItems = await StateManager.GetOrAddAsync<IReliableDictionary<UserSessionItemId, UserSessionItem>>(SessionKeyDictionaryName);

            ServiceEventSource.Current.Message($"Method started {nameof(UserSessionService)}->GetSessionItem() at: {DateTime.UtcNow}");
            UserSessionItem result = null;

            using (var tx = StateManager.CreateTransaction())
            {
                var enumerator = (await sessionKeyItems.CreateEnumerableAsync(tx)).GetAsyncEnumerator();
                while (await enumerator.MoveNextAsync(cancellationToken))
                {
                    if (enumerator.Current.Key == sessionKeyItemId)
                    {
                        result = enumerator.Current.Value;
                        break;
                    }
                }
            }
            ServiceEventSource.Current.Message($"Method ended {nameof(UserSessionService)}->GetSessionItem() at: {DateTime.UtcNow}");
            return result;
        }

        public Task RemoveSessionItem(UserSessionItemId sessionKeyItemId, CancellationToken cancellationToken)
        {
            // For now I am removing data via run async, but this one also need to be implemented if user wants to delete the session explicitly 

            return null;
        }

        public async Task SetSessionItem(UserSessionItem sessionKeyItem, CancellationToken cancellationToken)
        {
            var sessionKeyItems = await StateManager.GetOrAddAsync<IReliableDictionary<UserSessionItemId, UserSessionItem>>(SessionKeyDictionaryName);

            ServiceEventSource.Current.Message($"Method started {nameof(UserSessionService)}->SetSessionItem() at: {DateTime.UtcNow}");
            using (var tx = StateManager.CreateTransaction())
            {
                await sessionKeyItems.AddOrUpdateAsync(tx, sessionKeyItem.Id, sessionKeyItem, (key, value) => sessionKeyItem);
                await tx.CommitAsync();
            }
            ServiceEventSource.Current.Message($"Method ended {nameof(UserSessionService)}->SetSessionItem() at: {DateTime.UtcNow}");
        }

        public UserSessionService(StatefulServiceContext serviceContext)
            : this(serviceContext, new ReliableStateManager(serviceContext))
        {
        }
        public UserSessionService(StatefulServiceContext context, IReliableStateManagerReplica stateManagerreplica)
            : base(context, stateManagerreplica)
        {
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // We are going to loop to remove old session data            

            var sessionKeyItems = await StateManager.GetOrAddAsync<IReliableDictionary<UserSessionItemId, UserSessionItem>>(SessionKeyDictionaryName);

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var now = DateTimeOffset.Now.ToUniversalTime().ToUnixTimeSeconds();
                    var enumerator = (await sessionKeyItems.CreateEnumerableAsync(tx)).GetAsyncEnumerator();
                    while (await enumerator.MoveNextAsync(cancellationToken))
                    {
                        var userSessionItem = enumerator.Current.Value;
                        var userSessionItemId = enumerator.Current.Key;

                        if (userSessionItem.Ttl > now)
                        {
                            await sessionKeyItems.TryRemoveAsync(tx, userSessionItemId);
                        }
                    }

                    await tx.CommitAsync();
                }

                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
        }

    }
}
