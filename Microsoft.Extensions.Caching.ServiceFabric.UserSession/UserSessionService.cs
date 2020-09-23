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
        public Task RemoveSessionItem(SessionKeyItemId sessionKeyItemId, CancellationToken cancellationToken) { return null; }

        /*
          public async Task<byte[]> GetSessionItem(string key, CancellationToken cancellationToken)
          {
              var stateValue = await StateManager.TryGetStateAsync<byte[]>(key, cancellationToken);
              return stateValue.HasValue ? stateValue.Value : null;
          }                
               
          */

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

        private const string SessionKeyDictionaryName = "Microsoft.Extensions.Caching.ServiceFabric.SessionKeys.ProtectionKeys";

        public UserSessionService(StatefulServiceContext serviceContext)
            : this(serviceContext, new ReliableStateManager(serviceContext))
        {
        }
        public UserSessionService(StatefulServiceContext context, IReliableStateManagerReplica stateManagerreplica)
            : base(context, stateManagerreplica)
        {
        }

        //public async Task<long> GetKeyCountAsync(CancellationToken cancellationToken)
        //{
        //    //var sessionKeyItems = await StateManager.GetOrAddAsync<IReliableDictionary<SessionKeyItemId, SessionKeyItem>>(SessionKeyDictionaryName);

        //    //ServiceEventSource.Current.Message($"Method started {nameof(SessionKeys)}->GetKeyCountAsync() at: {DateTime.UtcNow}");

        //    //long sessionKeyCount = 0;

        //    //if (sessionKeyItems != null)
        //    //{
        //    //    using (var tx = StateManager.CreateTransaction())
        //    //    {
        //    //        sessionKeyCount = await sessionKeyItems.GetCountAsync(tx);
        //    //    }
        //    //}
        //    //ServiceEventSource.Current.Message($"Method ended {nameof(SessionKeys)}->GetKeyCountAsync() at: {DateTime.UtcNow}");
        //    //return sessionKeyCount;
        //}

        //public async Task AddKey(SessionKeyItem keyItem)
        //{
        //    //var sessionKeyItems = await StateManager.GetOrAddAsync<IReliableDictionary<SessionKeyItemId, SessionKeyItem>>(SessionKeyDictionaryName);

        //    //ServiceEventSource.Current.Message($"Method started {nameof(SessionKeys)}->AddKey() at: {DateTime.UtcNow}");
        //    //using (var tx = StateManager.CreateTransaction())
        //    //{
        //    //    await sessionKeyItems.AddOrUpdateAsync(tx, keyItem.Id, keyItem, (key, value) => keyItem);
        //    //    await tx.CommitAsync();
        //    //}
        //    //ServiceEventSource.Current.Message($"Method ended {nameof(SessionKeys)}->AddKey() at: {DateTime.UtcNow}");
        //}

        //public async Task<IEnumerable<SessionKeyItem>> GetKeys(CancellationToken cancellationToken)
        //{
        //    //var sessionKeyItems = await StateManager.GetOrAddAsync<IReliableDictionary<SessionKeyItemId, SessionKeyItem>>(SessionKeyDictionaryName);

        //    //ServiceEventSource.Current.Message($"Method started {nameof(SessionKeys)}->GetKeys() at: {DateTime.UtcNow}");
        //    //var results = new List<SessionKeyItem>();
        //    //using (var tx = StateManager.CreateTransaction())
        //    //{
        //    //    var enumerator = (await sessionKeyItems.CreateEnumerableAsync(tx)).GetAsyncEnumerator();
        //    //    while (await enumerator.MoveNextAsync(cancellationToken))
        //    //    {
        //    //        results.Add(enumerator.Current.Value);
        //    //    }
        //    //}
        //    //ServiceEventSource.Current.Message($"Method ended {nameof(SessionKeys)}->GetKeys() at: {DateTime.UtcNow}");
        //    //return results;

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }
    }
}
