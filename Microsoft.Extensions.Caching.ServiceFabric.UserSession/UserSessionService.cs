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



//TODO: remove actros and replace it with RC because same user can access session from different threads.

namespace Microsoft.Extensions.Caching.ServiceFabric.UserSession
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated. ?
    /// </remarks>
    internal class UserSessionService : StatefulService, IUserSessionService
    {
        private const string SessionKeyDictionaryName = "Microsoft.Extensions.Caching.ServiceFabric.SessionKeys.UserSessionService";

        public async Task<SessionKeyItem> GetSessionItem(SessionKeyItemId sessionKeyItemId, CancellationToken cancellationToken)
        {

            var sessionKeyItems = await StateManager.GetOrAddAsync<IReliableDictionary<SessionKeyItemId, SessionKeyItem>>(SessionKeyDictionaryName);

            ServiceEventSource.Current.Message($"Method started {nameof(UserSessionService)}->GetSessionItem() at: {DateTime.UtcNow}");
            SessionKeyItem result = null;

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
        public Task RemoveSessionItem(SessionKeyItemId sessionKeyItemId, CancellationToken cancellationToken) 
        {
            return null; 
        }

        public async Task SetSessionItem(SessionKeyItem sessionKeyItem, CancellationToken cancellationToken)
        {
            var sessionKeyItems = await StateManager.GetOrAddAsync<IReliableDictionary<SessionKeyItemId, SessionKeyItem>>(SessionKeyDictionaryName);

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
    }
}
