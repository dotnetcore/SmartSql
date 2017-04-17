using SmartSql.Abstractions.Cache;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using StackExchange.Redis;
using Newtonsoft.Json;

namespace SmartSql.Cache.Redis
{
    public class RedisCacheProvider : ICacheProvider
    {
        private ConnectionMultiplexer connection;
        private int databaseId =0;
        private IDatabase cacheDB { get { return connection.GetDatabase(databaseId); } }
        public void Initialize(IDictionary properties)
        {
            String connStr = properties["ConnectionString"]?.ToString();
            if (String.IsNullOrEmpty(connStr))
            {
                throw new Exception("SmartSql.Cache.Redis.ConnectionString string can't empty.");
            }
            connection = ConnectionMultiplexer.Connect(connStr);
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
            //var endPoint = connection.GetEndPoints()[0];
            //var dbServer = connection.GetServer(endPoint);
            //dbServer.FlushDatabase(databaseId);
            cacheDB.KeyDelete("*");
        }

        public bool Remove(CacheKey key)
        {
            return cacheDB.KeyDelete(key.Key);
        }
    }
}
