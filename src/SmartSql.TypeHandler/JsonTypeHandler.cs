using Newtonsoft.Json;
using SmartSql.TypeHandlers;
using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json.Serialization;
using SmartSql.Data;

namespace SmartSql.TypeHandler
{
    public class JsonTypeHandler : JsonTypeHandler<Object>
    {
        public override void Initialize(IDictionary<string, object> parameters)
        {
            base.Initialize(parameters);
        }
        public override Object GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            if (dataReader.IsDBNull(columnIndex)) { return null; }
            var jsonStr = dataReader.GetString(columnIndex);
            return JsonConvert.DeserializeObject(jsonStr, targetType, JsonSerializerSettings);
        }

        public override void SetParameter(IDataParameter dataParameter, object parameterValue)
        {
            if (parameterValue == null)
            {
                dataParameter.Value = DBNull.Value;
            }
            else
            {
                dataParameter.Value = JsonConvert.SerializeObject(parameterValue, JsonSerializerSettings);
            }
        }
    }
}
