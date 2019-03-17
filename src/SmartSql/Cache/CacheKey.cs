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
        public Type ResultType { get;}

        public CacheKey(RequestContext requestContext)
        {
            ResultType = requestContext.ExecutionContext.Result.ResultType;
            StatementKey = requestContext.IsStatementSql ? requestContext.FullSqlId : requestContext.RealSql;
            StringBuilder keyBuilder = new StringBuilder();
            keyBuilder.AppendFormat("{0}:DbParams=", StatementKey);
            if (requestContext.Parameters.DbParameters == null)
            {
                keyBuilder.Append("NULL");
            }
            foreach (var dbParam in requestContext.Parameters.DbParameters)
            {
                keyBuilder.AppendFormat("{0}={1}&", dbParam.Key, dbParam.Value.Value ?? "NULL");
            }
            Key = keyBuilder.ToString();
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
