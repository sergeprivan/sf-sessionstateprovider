using Microsoft.ServiceFabric.Services.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Caching.ServiceFabric.UserSession.Interfaces
{
    [Serializable]
    public class UserSessionItem
    {
        public UserSessionItem(string key, byte[] value, UserSessionItemId id = null)
        {
            Id = id ?? new UserSessionItemId();
            Key = key;
            Value = value;
        }

        public UserSessionItemId Id { get; }
        public string Key { get; }
        public byte[] Value { get; }

        public DateTime CreateDate { get; set; }
        public string ExpiryType { get; set; }
        public long Epoctime { get; set; }
        public long Ttl { get; set; }
        public override string ToString()
        {
            return $"Session Key: {Key} with Value: {Value} at: {DateTime.UtcNow}";
        }

        public ServicePartitionKey GetPartitionKey()
        {
            return new ServicePartitionKey(HashUtil.GetLongHashCode(Key));
        }
    }
}
