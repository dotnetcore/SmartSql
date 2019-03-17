using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Cache;

namespace SmartSql.DistributedCache
{
    public class MD5CacheKeyGegenerator : ICacheKeyGegenerator
    {
        private const string MD5_SALT = "SmartSql";
        public string Gegenerate(CacheKey cacheKey)
        {
            string strKey = $"{cacheKey.Key}-{cacheKey.ResultType.FullName}";
            return MD5Util.Encrypt(strKey, MD5_SALT);
        }
    }
}
