using SmartSql.Abstractions.Cache;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using StackExchange.Redis;
using Newtonsoft.Json;
using SmartSql.Exceptions;

namespace SmartSql.Cache.Redis
{
    public class RedisCacheProvider : ICacheProvider
    {
        private String _connStr;
        private ConnectionMultiplexer _redis;
        private int _databaseId = 0;
        private IDatabase CacheDB { get { return _redis.GetDatabase(_databaseId); } }
        private String _prefix;

        public void Initialize(IDictionary properties)
        {
            _connStr = properties["ConnectionString"]?.ToString();
            if (String.IsNullOrEmpty(_connStr))
            {
                throw new SmartSqlException("SmartSql.Cache.Redis.ConnectionString string can't empty.");
            }

            String databaseIdStr = properties["DatabaseId"]?.ToString();
            if (String.IsNullOrEmpty(databaseIdStr))
            {
                throw new SmartSqlException("SmartSql.Cache.Redis.DatabaseId string can't empty.");
            }
            else
            {
                if (!Int32.TryParse(databaseIdStr, out _databaseId))
                {
                    throw new SmartSqlException("SmartSql.Cache.Redis.DatabaseId string is not int.");
                }
            }
            _prefix = properties["Prefix"]?.ToString();
            if (String.IsNullOrEmpty(_prefix))
            {
                throw new SmartSqlException("SmartSql.Cache.Redis.Prefix string can't empty.");
            }

            _redis = RedisManager.Instance.GetRedis(_connStr);
        }

        public object this[CacheKey key, Type type]
        {
            get
            {
                string cacheStr = CacheDB.StringGet(key.Key);
                if (String.IsNullOrEmpty(cacheStr)) { return null; }
                return JsonConvert.DeserializeObject(cacheStr, type);
            }
            set
            {
                string cacheStr = JsonConvert.SerializeObject(value);
                CacheDB.StringSet(key.Key, cacheStr);
            }
        }

        public void Flush()
        {
            var serverEndPoint = _redis.GetEndPoints()[0];
            IServer servier = _redis.GetServer(serverEndPoint);
            var keys = servier.Keys(_databaseId, pattern: $"{_prefix}*");
            foreach (string key in keys)
            {
                CacheDB.KeyDelete(key);
            }
        }

        public bool Remove(CacheKey key)
        {
            return CacheDB.KeyDelete(key.Key);
        }

        public bool Contains(CacheKey key)
        {
            return CacheDB.KeyExists(key.Key);
        }
    }
}
