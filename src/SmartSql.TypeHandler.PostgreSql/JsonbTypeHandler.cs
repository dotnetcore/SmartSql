using SmartSql.Abstractions.TypeHandler;
using System;
using System.Data;
using Newtonsoft.Json;

namespace SmartSql.TypeHandler.PostgreSql
{
    public class JsonbTypeHandler : JsonTypeHandler
    {
        public override object SetParameter(IDataParameter dataParameter, object parameterValue)
        {
            var npgParam = dataParameter as Npgsql.NpgsqlParameter;
            npgParam.Value = GetSetParameterValue(parameterValue);
            npgParam.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Jsonb;
            return dataParameter.Value;
        }
    }
}
