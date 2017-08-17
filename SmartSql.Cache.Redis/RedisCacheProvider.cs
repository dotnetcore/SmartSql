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
        private String connStr;
        private ConnectionMultiplexer redis;
        private int databaseId = 0;
        private IDatabase cacheDB { get { return redis.GetDatabase(databaseId); } }
        private String prefix;

        public void Initialize(IDictionary properties)
        {
            connStr = properties["ConnectionString"]?.ToString();
            if (String.IsNullOrEmpty(connStr))
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
                if (!Int32.TryParse(databaseIdStr, out databaseId))
                {
                    throw new SmartSqlException("SmartSql.Cache.Redis.DatabaseId string is not int.");
                }
            }
            prefix = properties["Prefix"]?.ToString();
            if (String.IsNullOrEmpty(prefix))
            {
                throw new SmartSqlException("SmartSql.Cache.Redis.Prefix string can't empty.");
            }

            redis = RedisManager.Instance.GetRedis(connStr);
        }

        public object this[CacheKey key, Type type]
        {
            get
            {
                string cacheStr = cacheDB.StringGet(key.Key);
                if (String.IsNullOrEmpty(cacheStr)) { return null; }
                return JsonConvert.DeserializeObject(cacheStr, type);
            }
            set
            {
                string cacheStr = JsonConvert.SerializeObject(value);
                cacheDB.StringSet(key.Key, cacheStr);
            }
        }

        public void Flush()
        {
            var serverEndPoint = redis.GetEndPoints()[0];
            IServer servier = redis.GetServer(serverEndPoint);
            var keys = servier.Keys(databaseId, pattern: $"{prefix}*");
            foreach (string key in keys)
            {
                cacheDB.KeyDelete(key);
            }
        }

        public bool Remove(CacheKey key)
        {
            return cacheDB.KeyDelete(key.Key);
        }
    }
}
