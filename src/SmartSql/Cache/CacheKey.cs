using System;
using System.Security.Cryptography;
using System.Text;

namespace SmartSql.Cache
{
    /// <summary>
    /// TODO HASH collision
    /// </summary>
    public class CacheKey
    {
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
            var dbParameters = requestContext.Parameters.DbParameters?.Values;
            if (dbParameters != null && dbParameters.Count > 0)
            {
                StringBuilder keyBuilder = new StringBuilder();
                keyBuilder.Append(requestContext.RealSql);
                keyBuilder.Append("|?");
                foreach (var dbParam in dbParameters)
                {
                    keyBuilder.AppendFormat("{0}={1}&", dbParam.ParameterName, dbParam.Value ?? "NULL");
                }

                Key = keyBuilder.ToString().TrimEnd('&');
            }
            else
            {
                Key = requestContext.RealSql;
            }
            
            using (HashAlgorithm hashAlg = SHA256.Create())
            {
                var sqlBytes = UTF8Encoding.Default.GetBytes(Key);
                var hashData = hashAlg.ComputeHash(sqlBytes);
                Key = Convert.ToBase64String(hashData);
                if (requestContext.IsStatementSql)
                {
                    Key = $"{requestContext.FullSqlId}:{Key}";
                }
            }
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