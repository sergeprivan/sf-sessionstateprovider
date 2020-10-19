﻿using Microsoft.ServiceFabric.Services.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Caching.ServiceFabric.UserSession.Interfaces
{
    [Serializable]
    public class SessionKeyItem
    {
        public SessionKeyItem(string key, byte[] value, SessionKeyItemId id = null)
        {
            Id = id ?? new SessionKeyItemId();
            Key = key;
            Value = value;
        }

        public SessionKeyItemId Id { get; }
        public string Key { get; }
        public byte[] Value { get; }

        public override string ToString()
        {
            return string.Empty;
            //return $"Session Key: {Key} with Value: {Value} at: {DateTime.UtcNow}";
        }

        public ServicePartitionKey GetPartitionKey()
        {
            return new ServicePartitionKey(HashUtil.GetLongHashCode(Key));
        }
    }
}