using System;

namespace SmartSql.DyRepository.Annotations
{
    public class ResultCacheAttribute : Attribute
    {
        public ResultCacheAttribute(string cacheId)
        {
            CacheId = cacheId;
        }
        public String CacheId { get; }
        public String Key { get; set; }
    }
}