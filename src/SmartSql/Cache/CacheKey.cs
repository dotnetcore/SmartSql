using System;
using System.Collections.Generic;
using System.Linq;
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

        public CacheKey(string key, Type resultType)
        {
            Key = key;
            ResultType = resultType;
        }

        public CacheKey(AbstractRequestContext requestContext)
        {
            ResultType = requestContext.ExecutionContext.Result.ResultType;
            StatementKey = requestContext.IsStatementSql ? requestContext.FullSqlId : requestContext.RealSql;
            StringBuilder keyBuilder = new StringBuilder();
            keyBuilder.Append(StatementKey);
            var dbParameters = requestContext.Parameters.DbParameters?.Values;
            if (dbParameters != null && dbParameters.Count > 0)
            {
                keyBuilder.Append("?");
                foreach (var dbParam in dbParameters)
                {
                    keyBuilder.AppendFormat("{0}={1}&", dbParam.ParameterName, dbParam.Value ?? "NULL");
                }
            }

            Key = keyBuilder.ToString().TrimEnd('&');
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