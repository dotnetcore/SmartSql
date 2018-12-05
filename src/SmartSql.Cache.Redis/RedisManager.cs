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
            ConnectionMultiplexer redis;
            if (_redises.ContainsKey(connStr))
            {
                redis = _redises[connStr];
                if (redis.IsConnected)
                {
                    return redis;
                }
            }
            lock (this)
            {
                if (_redises.ContainsKey(connStr))
                {
                    redis = _redises[connStr];
                    if (redis.IsConnected)
                    {
                        return redis;
                    }
                }
                redis = ConnectionMultiplexer.Connect(connStr);
                if (_redises.ContainsKey(connStr))
                {
                    _redises[connStr] = redis;
                }
                else
                {
                    _redises.Add(connStr, redis);
                }

                return redis;
            }
        }
    }
}
