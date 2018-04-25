using SmartSql.Abstractions.Cache;
using SmartSql.Cache;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace SmartSql.Configuration
{
    public class CacheFactory
    {
        public static Cache Load(XmlElement cacheNode)
        {
            var cache = new Cache
            {
                Id = cacheNode.Attributes["Id"].Value,
                Type = cacheNode.Attributes["Type"].Value,
                Parameters = new Dictionary<String, String>(),
                FlushOnExecutes = new List<FlushOnExecute>()
            };
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
                            string hours = childNode.Attributes["Hours"]?.Value;
                            string minutes = childNode.Attributes["Minutes"]?.Value;
                            string seconds = childNode.Attributes["Seconds"]?.Value;
                            cache.FlushInterval = new FlushInterval
                            {
                                Hours = XmlConvert.ToInt32(hours),
                                Minutes = XmlConvert.ToInt32(minutes),
                                Seconds = XmlConvert.ToInt32(seconds)
                            };
                            break;
                        }
                    case "FlushOnExecute":
                        {
                            string statementId = childNode.Attributes["Statement"]?.Value;
                            if (!String.IsNullOrEmpty(statementId))
                            {
                                cache.FlushOnExecutes.Add(new FlushOnExecute
                                {
                                    Statement = statementId
                                });
                            }
                            break;
                        }
                }
            }
            return cache;
        }
        public static ICacheProvider CreateCacheProvider(Cache cache)
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
