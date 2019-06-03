using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Cache
{
    public class CacheKey
    {
        /// <summary>
        /// StatementKey:ReusltType.FullName:QueryString
        /// </summary>
        public String StatementKey { get; }

        public String Key { get; }
        public Type ResultType { get; }

        public CacheKey(AbstractRequestContext requestContext)
        {
            ResultType = requestContext.ExecutionContext.Result.ResultType;
            StatementKey = requestContext.IsStatementSql ? requestContext.FullSqlId : requestContext.RealSql;
            StringBuilder keyBuilder = new StringBuilder();
            keyBuilder.Append(StatementKey);
            if (requestContext.Parameters != null && requestContext.Parameters.Count > 0)
            {
                keyBuilder.Append(":Params=");
                foreach (var dbParam in requestContext.Parameters)
                {
                    keyBuilder.AppendFormat("{0}={1}&", dbParam.Key, dbParam.Value.Value ?? "NULL");
                }
                Key = keyBuilder.ToString().TrimEnd('&');
            }

            Key = keyBuilder.ToString();
        }

        public override string ToString() => Key;
        public override int GetHashCode() => Key.GetHashCode();

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (!(obj is CacheKey)) return false;
            CacheKey cacheKey = (CacheKey) obj;
            return cacheKey.Key == Key;
        }
    }
}