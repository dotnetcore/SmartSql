using System;
using SmartSql.Cache.Default;
using SmartSql.Configuration;
using SmartSql.Reflection;
using SmartSql.Reflection.ObjectFactoryBuilder;

namespace SmartSql.Cache
{
    public class CacheProviderUtil
    {
        public static ICacheProvider Create(Configuration.Cache cache)
        {
            ICacheProvider cacheProvider = null;
            switch (cache.Type)
            {
                case "Lru":
                {
                    cacheProvider = new LruCacheProvider();
                    break;
                }

                case "Fifo":
                {
                    cacheProvider = new FifoCacheProvider();
                    break;
                }

                default:
                {
                    Type cacheProviderType = TypeUtils.GetType(cache.Type);
                    cacheProvider = EmitObjectFactoryBuilder.Instance
                        .GetObjectFactory(cacheProviderType, Type.EmptyTypes)(null) as ICacheProvider;
                    break;
                }
            }

            cacheProvider.Initialize(cache.Parameters);
            return cacheProvider;
        }
    }
}