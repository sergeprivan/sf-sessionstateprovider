using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.Extensions.Caching.ServiceFabric.UserSession.Interfaces;

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
    [StatePersistence(StatePersistence.Persisted)]
    internal class UserSession : Actor, IUserSession
    {
        /// <summary>
        /// Initializes a new instance of UserSession
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public UserSession(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        public async Task<byte[]> GetSessionItem(string key, CancellationToken cancellationToken)
        {
            var stateValue = await StateManager.TryGetStateAsync<byte[]>(key, cancellationToken);
            return stateValue.HasValue ? stateValue.Value : null;
        }


        public async Task RemoveSessionItem(string key, CancellationToken cancellationToken)
        {
            await StateManager.TryRemoveStateAsync(key, cancellationToken);
        }

        public Task SetSessionItem(string key, byte[] value, CancellationToken cancellationToken)
        {
            return StateManager.AddOrUpdateStateAsync(key, value, (k, v) => value, cancellationToken);
        }
    }
}
