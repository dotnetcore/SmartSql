using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;
using System.Collections;
using Dapper;
using SmartSql.SqlMap.Tags;

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
                if (RequestContext.RequestParameters == null) { return "Null"; }
                StringBuilder strBuilder = new StringBuilder();
                var reqParams = RequestContext.RequestParameters;
                var paramNames = reqParams.ParameterNames.Where(p => !p.Contains(For.FOR_KEY_SUFFIX)).ToList().OrderBy(p => p);
                foreach (var paramName in paramNames)
                {
                    var val = reqParams.Get<object>(paramName);
                    BuildSqlQueryString(strBuilder, paramName, val);
                }
                return strBuilder.ToString().Trim('&');
            }
        }

        private void BuildSqlQueryString(StringBuilder strBuilder, string key, object val)
        {
            if (val is IEnumerable list && !(val is String))
            {
                strBuilder.AppendFormat("&{0}=(", key);
                foreach (var item in list)
                {
                    strBuilder.AppendFormat("{0},", item);
                }
                strBuilder.Append(")");
            }
            else
            {
                strBuilder.AppendFormat("&{0}={1}", key, val);
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
