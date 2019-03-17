using Newtonsoft.Json;
using SmartSql.TypeHandlers;
using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json.Serialization;
using SmartSql.Data;

namespace SmartSql.TypeHandler
{
    public class JsonTypeHandler<T> : AbstractNullableTypeHandler<T>
    {
        protected JsonSerializerSettings JsonSerializerSettings { get; }

        public JsonTypeHandler()
        {
            JsonSerializerSettings = new JsonSerializerSettings();
        }

        public override void Initialize(IDictionary<string, object> parameters)
        {
            if (parameters.Value<string, object, string>("DateFormat", out var dateFormatStr))
            {
                JsonSerializerSettings.DateFormatString = dateFormatStr;
            }
            if (parameters.Value<string, object, string>("NamingStrategy", out var namingStrategyStr))
            {
                var defaultContractResolver = new DefaultContractResolver();
                switch (namingStrategyStr)
                {
                    case "Camel":
                        {
                            defaultContractResolver.NamingStrategy = new CamelCaseNamingStrategy();
                            break;
                        }
                    case "Snake":
                        {
                            defaultContractResolver.NamingStrategy = new SnakeCaseNamingStrategy();
                            break;
                        }
                    default:
                        {
                            defaultContractResolver.NamingStrategy = new DefaultNamingStrategy();
                            break;
                        }
                }

                JsonSerializerSettings.ContractResolver = defaultContractResolver;
            }
            base.Initialize(parameters);
        }
        public override T GetValue(DataReaderWrapper dataReader, int columnIndex)
        {
            if (dataReader.IsDBNull(columnIndex)) { return default(T); }
            var jsonStr = dataReader.GetString(columnIndex);
            return JsonConvert.DeserializeObject<T>(jsonStr, JsonSerializerSettings);
        }

        public override T GetValue(DataReaderWrapper dataReader, string columnName)
        {
            int ordinal = dataReader.GetOrdinal(columnName);
            return GetValue(dataReader, ordinal);
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
