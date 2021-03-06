﻿using Microsoft.Extensions.Options;
using System;

namespace Microsoft.Extensions.Caching.ServiceFabric
{
    public class ServiceFabricCacheOptions : IOptions<ServiceFabricCacheOptions>
    {
        public TimeSpan IdleTimeout { get; set; } = new TimeSpan(0, 20, 0);
        public string TtlAttribute { get; set; } = "TTL";
        public ISessionService SessionService { get; set; }

        ServiceFabricCacheOptions IOptions<ServiceFabricCacheOptions>.Value
        {
            get { return this; }
        }
    }
}
