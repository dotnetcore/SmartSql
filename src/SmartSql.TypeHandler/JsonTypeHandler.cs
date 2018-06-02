using SmartSql.Abstractions.TypeHandler;
using System;
using System.Data;
using Newtonsoft.Json;

namespace SmartSql.TypeHandler
{
    public class JsonTypeHandler : ITypeHandler
    {
        public virtual object GetValue(IDataReader dataReader, string columnName, Type targetType)
        {
            int ordinal = dataReader.GetOrdinal(columnName);
            return GetValue(dataReader, ordinal, targetType);
        }

        public virtual object GetValue(IDataReader dataReader, int columnIndex, Type targetType)
        {
            var jsonStr = dataReader.GetString(columnIndex);
            if (String.IsNullOrEmpty(jsonStr)) { return null; }
            return JsonConvert.DeserializeObject(jsonStr, targetType);
        }

        public virtual void SetParameter(IDataParameter dataParameter, object parameterValue)
        {
            if (parameterValue == null)
            {
                dataParameter.Value = DBNull.Value;
            }
            else
            {
                var jsonStr = JsonConvert.SerializeObject(parameterValue);
                dataParameter.Value = jsonStr;
            }
        }
    }
}
