using SmartSql.Abstractions.Cache;
using SmartSql.Cache;
using SmartSql.Configuration.Maps;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace SmartSql.Configuration
{
    public class CacheFactory
    {
        public static Cache Load(XmlElement cacheNode, SmartSqlMap sqlMap)
        {
            var cache = new Cache
            {
                Id = cacheNode.Attributes["Id"].Value,
                Type = cacheNode.Attributes["Type"].Value,
                Parameters = new Dictionary<String, String>(),
                FlushOnExecutes = new List<FlushOnExecute>()
            };
            if (cache.Id.IndexOf('.') < 0)
            {
                cache.Id = $"{sqlMap.Scope}.{cache.Id}";
            }
            foreach (XmlNode childNode in cacheNode.ChildNodes)
            {
                switch (childNode.Name)
                {
                    case "Parameter":
                        {
                            string key = childNode.Attributes["Key"]?.Value;
                            string val = childNode.Attributes["Value"]?.Value;
                            if (!String.IsNullOrEmpty(key))
                            {
                                cache.Parameters.Add(key, val);
                            }
                            break;
                        }
                    case "FlushInterval":
                        {
                            string hoursStr = childNode.Attributes["Hours"]?.Value;
                            string minutesStr = childNode.Attributes["Minutes"]?.Value;
                            string secondsStr = childNode.Attributes["Seconds"]?.Value;
                            int.TryParse(hoursStr, out int hours);
                            int.TryParse(minutesStr, out int minutes);
                            int.TryParse(secondsStr, out int seconds);
                            cache.FlushInterval = new FlushInterval
                            {
                                Hours = hours,
                                Minutes = minutes,
                                Seconds = seconds
                            };
                            break;
                        }
                    case "FlushOnExecute":
                        {
                            string statementId = childNode.Attributes["Statement"]?.Value;
                            if (!String.IsNullOrEmpty(statementId))
                            {
                                if (statementId.IndexOf('.') < 0)
                                {
                                    statementId = $"{sqlMap.Scope}.{statementId}";
                                }
                                cache.FlushOnExecutes.Add(new FlushOnExecute
                                {
                                    Statement = statementId
                                });
                            }
                            break;
                        }
                }
            }
            cache.Provider = CreateCacheProvider(cache);
            return cache;
        }
        private static ICacheProvider CreateCacheProvider(Cache cache)
        {
            ICacheProvider _cacheProvider = null;
            switch (cache.Type)
            {
                case "Lru":
                    {
                        _cacheProvider = new LruCacheProvider();
                        break;
                    }
                case "Fifo":
                    {
                        _cacheProvider = new FifoCacheProvider();
                        break;
                    }
                default:
                    {
                        var assName = new AssemblyName { Name = cache.AssemblyName };
                        Type _cacheProviderType = Assembly.Load(assName).GetType(cache.TypeName);
                        _cacheProvider = Activator.CreateInstance(_cacheProviderType) as ICacheProvider;
                        break;
                    }
            }
            _cacheProvider.Initialize(cache.Parameters);
            return _cacheProvider;
        }
    }
}
