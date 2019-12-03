using Microsoft.Extensions.Options;
using System;

namespace Microsoft.Extensions.Caching.ServiceFabric
{
    public class DynamoDbCacheOptions : IOptions<DynamoDbCacheOptions>
    {
        public string TableName { get; set; } = "ASP.NET_SessionState";
        public TimeSpan IdleTimeout { get; set; } = new TimeSpan(0, 20, 0);
        public string TtlAttribute { get; set; } = "TTL";

        DynamoDbCacheOptions IOptions<DynamoDbCacheOptions>.Value
        {
            get { return this; }
        }
    }
}
