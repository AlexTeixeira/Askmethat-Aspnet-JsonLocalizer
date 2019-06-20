using System;
using System.Collections.Generic;
using Askmethat.Aspnet.JsonLocalizer.Format;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Askmethat.Aspnet.JsonLocalizer.Caching
{
    internal sealed class CacheHelper
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IMemoryCache _memoryCache;

        public CacheHelper(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public CacheHelper(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public bool TryGetValue(string cacheKey, out Dictionary<string, LocalizatedFormat> localization)
        {
            if (_memoryCache != null)
            {
                return _memoryCache.TryGetValue(cacheKey, out localization);
            }
            
            if (_distributedCache != null)
            {
                var json = _distributedCache.GetString(cacheKey);
                if (json == null)
                {
                    localization = null;
                    return false;
                }

                localization = JsonConvert.DeserializeObject<Dictionary<string, LocalizatedFormat>>(json);
                return true;
            }

            throw new InvalidOperationException("Both MemoryCache and DistributedCache are null");
        }

        public void Set(string cacheKey, Dictionary<string, LocalizatedFormat> localization, TimeSpan cacheDuration)
        {
            if (_memoryCache == null && _distributedCache == null)
            {
                throw new InvalidOperationException("Both MemoryCache and DistributedCache are null");
            }

            if (_memoryCache != null)
            {
                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(cacheDuration);

                _memoryCache.Set(cacheKey, localization, cacheEntryOptions);
            }

            if (_distributedCache != null)
            {
                var cacheEntryOptions = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(cacheDuration);

                var json = JsonConvert.SerializeObject(localization);
                _distributedCache.SetString(cacheKey, json, cacheEntryOptions);
            }
        }
    }
}
