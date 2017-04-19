using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Cache.Redis
{
    public class RedisManager
    {
        private static IDictionary<String, ConnectionMultiplexer> redises = new Dictionary<String, ConnectionMultiplexer>();

        public static ConnectionMultiplexer GetRedis(String connStr)
        {
            if (redises.ContainsKey(connStr))
            {
                return redises[connStr];
            }
            var redis = ConnectionMultiplexer.Connect(connStr);
            redises.Add(connStr, redis);
            return redis;
        }


    }
}
