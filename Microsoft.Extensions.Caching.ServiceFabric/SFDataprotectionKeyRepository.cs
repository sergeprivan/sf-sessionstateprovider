using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Fabric;
using System.Fabric.Query;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Caching.ServiceFabric;
using Microsoft.Extensions.Caching.ServiceFabric.SessionKeys.Interfaces;

namespace Microsoft.Extensions.Caching.ServiceFabric
{
    public class ServiceFabricDataProtectionRepository : IXmlRepository
    {
        // TODO share FabricClient for better performance https://stackoverflow.com/questions/37774965/how-to-enumerate-all-partitions-and-aggregate-results
        static FabricClient _fabricClient = new FabricClient();
        static Uri _keyServiceUri = new ServiceUriBuilder("Microsoft.Extensions.Caching.ServiceFabric.SessionKeys").ToUri();

        public ServiceFabricDataProtectionRepository()
        {
        }

        public IReadOnlyCollection<XElement> GetAllElements()
        {
            try
            {
                var results = new List<XElement>();

                foreach (var key in GetAllCurrentKeys())
                {
                    results.Add(XElement.Parse(key.Value));
                }

                return new ReadOnlyCollection<XElement>(results);
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.Message($"{nameof(ServiceFabricDataProtectionRepository)}->GetAllElements() failed to retrieve Data Protection Keys with error: {ex.ToString()}");
            }
            return null;
        }

        /// <summary>
        /// We need to fix here some point related to the id as id or id as GetPartitionKey()
        /// </summary>
        /// <param name="element"></param>
        /// <param name="friendlyName"></param>
        public void StoreElement(XElement element, string friendlyName)
        {
            try
            {
                var partitions = GetPartitions().GetAwaiter().GetResult();
                foreach (var partition in partitions)
                {
                    var newKey = new SessionKeyItem(friendlyName, element.ToString());
                    var keysService = ServiceProxy.Create<ISessionKeysService>(_keyServiceUri, newKey.Id.GetPartitionKey());                    
                    keysService.AddKey(newKey);
                }
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.Message($"{nameof(ServiceFabricDataProtectionRepository)}->StoreElement() failed to save Data Protection Keys with error: {ex.ToString()}");
            }
        }

        private IEnumerable<SessionKeyItem> GetAllCurrentKeys()
        {
            var keys = new List<SessionKeyItem>();
            var partitions = GetPartitions().GetAwaiter().GetResult();
            foreach (var partition in partitions)
            {
                long minKey = (partition.PartitionInformation as Int64RangePartitionInformation).LowKey;
                var keysService = ServiceProxy.Create<ISessionKeysService>(_keyServiceUri, new ServicePartitionKey(minKey));
                var partitionKeys = keysService.GetKeys(CancellationToken.None).GetAwaiter().GetResult();
                keys.AddRange(partitionKeys);
            }
            return keys;
        }

        private async Task<ServicePartitionList> GetPartitions()
        {
            return await _fabricClient.QueryManager.GetPartitionListAsync(_keyServiceUri);
        }
    }
}
