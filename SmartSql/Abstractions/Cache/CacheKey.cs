using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Abstractions.Cache
{
    public class CacheKey
    {
        /// <summary>
        /// 缓存前缀
        /// </summary>
        public String Prefix { get; set; } = "SmartSql-Cache";
        public IRequestContext RequestContext { get; private set; }
        public CacheKey(RequestContext context)
        {
            RequestContext = context;
        }
        public override string ToString()
        {
            string key = $"{Prefix}:{RequestContext.FullSqlId}:{RequestContext.Request}";//此处 RequestContext.Request 需重新处理
            return key;
        }
    }
}
