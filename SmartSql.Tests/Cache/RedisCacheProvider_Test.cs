using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Cache.Redis;
using Xunit;
using SmartSql.Abstractions.Cache;

namespace SmartSql.Tests.Cache
{
    public class RedisCacheProvider_Test
    {
        RedisCacheProvider provider = new RedisCacheProvider();
        public RedisCacheProvider_Test()
        {

            Dictionary<String, String> properties = new Dictionary<string, string>();
            properties.Add("ConnectionString", "192.168.31.103");
            properties.Add("DatabaseId", "0");
            properties.Add("Prefix", "T_Test.GetListByCache");
            provider.Initialize(properties);
        }
        [Fact]
        public void Set()
        {
            CacheKey cacheKey = new CacheKey(new RequestContext
            {
                Scope = "T_Test",
                SqlId = "GetListByCache",
                Request = new { Id = 1, Name = "Ahoo" }
            });
            provider[cacheKey, typeof(IEnumerable<T_Test>)] = new List<T_Test>();
        }

        [Fact]
        public void Get()
        {
            CacheKey cacheKey = new CacheKey(new RequestContext
            {
                Scope = "T_Test",
                SqlId = "GetListByCache",

                Request = new { Id = 1, Name = "Ahoo" }
            });
            var result = provider[cacheKey, typeof(IEnumerable<T_Test>)];
            Assert.NotNull(result);
        }
        [Fact]
        public void Flush()
        {
            provider.Flush();
        }
    }
}
