using SmartSql.Abstractions.TypeHandler;
using System;
using System.Data;
using Newtonsoft.Json;

namespace SmartSql.TypeHandler.PostgreSql
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
            if (dataReader.IsDBNull(columnIndex)) { return null; }
            var jsonStr = dataReader.GetString(columnIndex);
            return JsonConvert.DeserializeObject(jsonStr, targetType);
        }
        public object GetSetParameterValue(object parameterValue)
        {
            if (parameterValue == null)
            {
                return DBNull.Value;
            }
            else
            {
                return JsonConvert.SerializeObject(parameterValue);
            }
        }
        public virtual object SetParameter(IDataParameter dataParameter, object parameterValue)
        {
            var npgParam = dataParameter as Npgsql.NpgsqlParameter;
            npgParam.Value = GetSetParameterValue(parameterValue);
            npgParam.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Json;
            return dataParameter.Value;
        }
    }
}
