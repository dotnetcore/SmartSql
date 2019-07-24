using System.Collections.Generic;
using System.Data.Common;

namespace SmartSql.Data
{
    public interface ISqlParameterCollection : IDictionary<string, SqlParameter>
    {
        bool IgnoreCase { get; }
        IDictionary<string, DbParameter> DbParameters { get; }
        void Add(SqlParameter sqlParameter);
        bool TryAdd(string key, SqlParameter sqlParameter);
        bool TryAdd(SqlParameter sqlParameter);
        bool TryAdd(string propertyName, object paramVal);
        bool TryGetParameterValue<T>(string propertyName, out T paramVal);
    }
}