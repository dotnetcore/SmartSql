using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;
namespace SmartSql.Abstractions.Cache
{
    public class CacheKey
    {
        /// <summary>
        /// 缓存前缀
        /// </summary>
        public String Prefix { get; set; } = "SmartSql-Cache";
        public RequestContext RequestContext { get; private set; }
        public String RequestQueryString
        {
            get
            {
                if (RequestContext.Request == null) { return "Null"; }
                var properties = RequestContext.Request.GetType().GetProperties().ToList().OrderBy(p => p.Name);
                StringBuilder strBuilder = new StringBuilder();
                foreach (var property in properties)
                {
                    var val = property.GetValue(RequestContext.Request);
                    strBuilder.AppendFormat("&{0}", val);
                }
                return strBuilder.ToString().Trim('&');
            }
        }
        public String Key { get { return $"{RequestContext.FullSqlId}:{RequestQueryString}"; } }
        public CacheKey(RequestContext context)
        {
            RequestContext = context;
        }
        public override string ToString() => Key;
        public override int GetHashCode() => Key.GetHashCode();
        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (!(obj is CacheKey)) return false;
            CacheKey cacheKey = (CacheKey)obj;
            return cacheKey.Key == Key;
        }
    }
}
