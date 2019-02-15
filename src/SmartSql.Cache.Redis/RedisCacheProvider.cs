using SmartSql.Abstractions.Cache;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using StackExchange.Redis;
using Newtonsoft.Json;
using SmartSql.Exceptions;
using System.Linq;
using SmartSql.Configuration;

namespace SmartSql.Cache.Redis
{
    public class RedisCacheProvider : ICacheProvider
    {
        private String _connStr;
        private ConnectionMultiplexer _redis => RedisManager.Instance.GetRedis(_connStr);
        private int _databaseId = 0;
        private IDatabase CacheDB { get { return _redis.GetDatabase(_databaseId); } }
        private String _prefix;
        private TimeSpan? _expiryInterval;
        public void Initialize(IDictionary properties)
        {
            _connStr = properties["ConnectionString"]?.ToString();
            if (String.IsNullOrEmpty(_connStr))
            {
                throw new SmartSqlException("SmartSql.Cache.Redis.ConnectionString string can't empty.");
            }
            _prefix = properties["Cache.Id"]?.ToString();

            String databaseIdStr = properties["DatabaseId"]?.ToString();
            if (!String.IsNullOrEmpty(databaseIdStr))
            {
                Int32.TryParse(databaseIdStr, out _databaseId);
            }
            var prefixStr = properties["Prefix"]?.ToString();
            if (!String.IsNullOrEmpty(prefixStr))
            {
                _prefix = prefixStr;
            }
            if (properties["FlushInterval"] is FlushInterval flushInterval)
            {
                _expiryInterval = flushInterval.Interval;
            }
        }

        private string GetRedisCacheKey(CacheKey key)
        {
            return $"{_prefix}:{key.Key}";
        }

        public object this[CacheKey key, Type type]
        {
            get
            {
                
                string cacheStr = CacheDB.StringGet(GetRedisCacheKey(key));
                if (String.IsNullOrEmpty(cacheStr)) { return null; }
                return JsonConvert.DeserializeObject(cacheStr, type);
            }
            set
            {
                string cacheStr = JsonConvert.SerializeObject(value);
                CacheDB.StringSet(GetRedisCacheKey(key), cacheStr, _expiryInterval);
            }
        }

        public void Flush()
        {
            var serverEndPoint = _redis.GetEndPoints()[0];
            IServer servier = _redis.GetServer(serverEndPoint);
            var keys = servier.Keys(_databaseId, pattern: $"{_prefix}*").ToArray();
            if (keys.Length > 0)
            {
                CacheDB.KeyDelete(keys.ToArray());
            }
        }

        public bool Remove(CacheKey key)
        {
            return CacheDB.KeyDelete(GetRedisCacheKey(key));
        }

        public bool Contains(CacheKey key)
        {
            return CacheDB.KeyExists(GetRedisCacheKey(key));
        }

        public void Dispose()
        {
            _redis.Dispose();
        }
    }
}
