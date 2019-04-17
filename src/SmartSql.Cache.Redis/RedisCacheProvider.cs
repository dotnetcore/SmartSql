using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SmartSql.Configuration;
using StackExchange.Redis;

namespace SmartSql.Cache.Redis
{
    public class RedisCacheProvider : ICacheProvider
    {
        private int _databaseId = 0;
        private string _prefix;
        private TimeSpan? _expiryInterval;
        private ConnectionMultiplexer _redis;
        private IDatabase _database;
        public void Initialize(IDictionary<string, object> properties)
        {
            properties.EnsureValue("ConnectionString", out string connStr);

            if (!properties.Value("Prefix", out _prefix))
            {
                properties.Value("Cache.Id", out _prefix);
            }
            properties.Value("DatabaseId", out _databaseId);
            if (properties.Value("FlushInterval", out FlushInterval flushInterval))
            {
                _expiryInterval = flushInterval.Interval;
            }
            _redis = ConnectionMultiplexer.Connect(connStr);
            _database = _redis.GetDatabase(_databaseId);
        }

        private string GetCacheKey(CacheKey cacheKey)
        {
            return $"{_prefix}:{cacheKey.Key}";
        }

        public bool TryAdd(CacheKey cacheKey, object cacheItem)
        {
            string cacheStr = JsonConvert.SerializeObject(cacheItem);
            return _database.StringSet(GetCacheKey(cacheKey), cacheStr, _expiryInterval);
        }

        public void Flush()
        {
            var serverEndPoint = _redis.GetEndPoints()[0];
            var server = _redis.GetServer(serverEndPoint);
            var keys = server.Keys(_databaseId, pattern: $"{_prefix}:*").ToArray();
            if (keys.Length > 0)
            {
                _database.KeyDelete(keys.ToArray());
            }
        }

        public bool TryGetValue(CacheKey cacheKey, out object cacheItem)
        {
            cacheItem = null;
            string cacheStr = _database.StringGet(GetCacheKey(cacheKey));
            if (String.IsNullOrEmpty(cacheStr)) { return false; }
            cacheItem = JsonConvert.DeserializeObject(cacheStr, cacheKey.ResultType);
            return true;
        }

        public void Dispose()
        {
            _redis.Dispose();
        }
    }
}
