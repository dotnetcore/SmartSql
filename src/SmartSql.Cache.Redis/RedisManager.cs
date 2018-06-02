using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Cache.Redis
{
    public class RedisManager
    {
        private RedisManager() { }
        public static RedisManager Instance = new RedisManager();

        private IDictionary<String, ConnectionMultiplexer> _redises = new Dictionary<String, ConnectionMultiplexer>();

        public ConnectionMultiplexer GetRedis(String connStr)
        {
            if (_redises.ContainsKey(connStr))
            {
                return _redises[connStr];
            }
            lock (this)
            {
                if (_redises.ContainsKey(connStr))
                {
                    return _redises[connStr];
                }
                var redis = ConnectionMultiplexer.Connect(connStr);
                _redises.Add(connStr, redis);
                return redis;
            }
        }
    }
}
