﻿using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Data;
using Microsoft.Extensions.Caching.ServiceFabric.SessionKeys.Interfaces;

namespace Microsoft.Extensions.Caching.ServiceFabric.SessionKeys
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class SessionKeys : StatefulService, ISessionKeysService
    {

        private const string SessionKeyDictionaryName = "Microsoft.Extensions.Caching.ServiceFabric.SessionKeys.ProtectionKeys";

        public SessionKeys(StatefulServiceContext serviceContext)
            : this(serviceContext, new ReliableStateManager(serviceContext))
        {
        }
        public SessionKeys(StatefulServiceContext context, IReliableStateManagerReplica stateManagerreplica)
            : base(context, stateManagerreplica)
        {
        }

        public async Task<long> GetKeyCountAsync(CancellationToken cancellationToken)
        {
            var sessionKeyItems = await StateManager.GetOrAddAsync<IReliableDictionary<SessionKeyItemId, SessionKeyItem>>(SessionKeyDictionaryName);

            ServiceEventSource.Current.Message($"Method started {nameof(SessionKeys)}->GetKeyCountAsync() at: {DateTime.UtcNow}");

            long sessionKeyCount = 0;

            if (sessionKeyItems != null)
            {
                using (var tx = StateManager.CreateTransaction())
                {
                    sessionKeyCount = await sessionKeyItems.GetCountAsync(tx);
                }
            }
            ServiceEventSource.Current.Message($"Method ended {nameof(SessionKeys)}->GetKeyCountAsync() at: {DateTime.UtcNow}");
            return sessionKeyCount;
        }
        public async Task AddKey(SessionKeyItem keyItem)
        {
            var sessionKeyItems = await StateManager.GetOrAddAsync<IReliableDictionary<SessionKeyItemId, SessionKeyItem>>(SessionKeyDictionaryName);

            ServiceEventSource.Current.Message($"Method started {nameof(SessionKeys)}->AddKey() at: {DateTime.UtcNow}");
            using (var tx = StateManager.CreateTransaction())
            {
                await sessionKeyItems.AddOrUpdateAsync(tx, keyItem.Id, keyItem, (key, value) => keyItem);
                await tx.CommitAsync();
            }
            ServiceEventSource.Current.Message($"Method ended {nameof(SessionKeys)}->AddKey() at: {DateTime.UtcNow}");
        }

        public async Task<IEnumerable<SessionKeyItem>> GetKeys(CancellationToken cancellationToken)
        {
            var sessionKeyItems = await StateManager.GetOrAddAsync<IReliableDictionary<SessionKeyItemId, SessionKeyItem>>(SessionKeyDictionaryName);

            ServiceEventSource.Current.Message($"Method started {nameof(SessionKeys)}->GetKeys() at: {DateTime.UtcNow}");
            var results = new List<SessionKeyItem>();
            using (var tx = StateManager.CreateTransaction())
            {
                var enumerator = (await sessionKeyItems.CreateEnumerableAsync(tx)).GetAsyncEnumerator();
                while (await enumerator.MoveNextAsync(cancellationToken))
                {
                    results.Add(enumerator.Current.Value);
                }
            }
            ServiceEventSource.Current.Message($"Method ended {nameof(SessionKeys)}->GetKeys() at: {DateTime.UtcNow}");
            return results;
        }
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
