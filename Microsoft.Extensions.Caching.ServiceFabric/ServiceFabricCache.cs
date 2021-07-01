using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.ServiceFabric.UserSession.Interfaces;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Caching.ServiceFabric
{
    public class ServiceFabricCache : IDistributedCache
    {
        //private string _tableName = "ASP.NET_SessionState";
        //private string _ttlfield = "TTL";
        private int _sessionMinutes = 20;
        private enum ExpiryType
        {
            Sliding,
            Absolute
        }
        ISessionService SessionService { get; }

        public ServiceFabricCache(IOptions<ServiceFabricCacheOptions> optionsAccessor)
        {
            SessionService = optionsAccessor.Value.SessionService ?? throw new ArgumentNullException(nameof(optionsAccessor.Value.SessionService));
        }

        public byte[] Get(string key)
        {
            var result = GetAsync(key).Result;
            return result;
        }

        public async Task<byte[]> GetAsync(string key, CancellationToken token = default(CancellationToken))
        {
            var result = await SessionService.GetSessionItem(key, key);
            return result.Value;
        }

        public void Refresh(string key)
        {
            RefreshAsync(key).Wait();
        }

        public async Task RefreshAsync(string key, CancellationToken token = default(CancellationToken))
        {
            //var value = _table.GetItemAsync(key).Result;
            //if (value == null || value["ExpiryType"] == null || value["ExpiryType"] != "Sliding")
            //{
            //    return;
            //}
            //value[_ttlfield] = DateTimeOffset.Now.ToUniversalTime().ToUnixTimeSeconds() + (_sessionMinutes * 60);


            await SetAsync(key, Get(key), new DistributedCacheEntryOptions { SlidingExpiration = new TimeSpan(0, _sessionMinutes, 0) });
        }

        public void Remove(string key)
        {
            RemoveAsync(key).Wait();
        }

        public async Task RemoveAsync(string key, CancellationToken token = default(CancellationToken))
        {
            var sessionItem = await SessionService.GetSessionItem(key, key);
            await SessionService.RemoveSessionItem(key, sessionItem);
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            SetAsync(key, value, options).Wait();
        }

        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))
        {
            var sessionItem = new UserSessionItem(key, value);
            await SessionService.AddSessionItem(key, sessionItem);
            //ExpiryType expiryType;
            //var epoctime = GetEpochExpiry(options, out expiryType);
            //var _ssdoc = new Document();

            //_ssdoc.Add("SessionId", key);
            //_ssdoc.Add("Session", value);
            //_ssdoc.Add("CreateDate", DateTime.Now.ToUniversalTime().ToString("o"));
            //_ssdoc.Add("ExpiryType", expiryType.ToString());
            //_ssdoc.Add(_ttlfield, epoctime);

            ////await _table.PutItemAsync(_ssdoc);
        }

        private long GetEpochExpiry(DistributedCacheEntryOptions options, out ExpiryType expiryType)
        {
            if (options.SlidingExpiration.HasValue)
            {
                expiryType = ExpiryType.Sliding;
                return DateTimeOffset.Now.ToUniversalTime().ToUnixTimeSeconds() + (long)options.SlidingExpiration.Value.TotalSeconds;

            }
            else if (options.AbsoluteExpiration.HasValue)
            {
                expiryType = ExpiryType.Absolute;
                return options.AbsoluteExpiration.Value.ToUnixTimeSeconds();

            }
            else if (options.AbsoluteExpirationRelativeToNow.HasValue)
            {
                expiryType = ExpiryType.Absolute;
                return DateTimeOffset.Now.Add(options.AbsoluteExpirationRelativeToNow.Value).ToUniversalTime().ToUnixTimeSeconds();
            }
            else
            {
                throw new Exception("Cache expiry option must be set to Sliding, Absolute or Absolute relative to now");
            }
        }
    }
}
