using Newtonsoft.Json;
using SmartSql.TypeHandlers;
using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json.Serialization;
using SmartSql.Data;

namespace SmartSql.TypeHandler
{
    public class JsonTypeHandler<TProperty> : AbstractNullableTypeHandler<TProperty, String>
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
        public override TProperty GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            if (dataReader.IsDBNull(columnIndex)) { return default(TProperty); }
            var jsonStr = dataReader.GetString(columnIndex);
            return (TProperty)JsonConvert.DeserializeObject(jsonStr, targetType, JsonSerializerSettings);
        }
        
        protected override object GetSetParameterValueWhenNotNull(object parameterValue)
        {
            return JsonConvert.SerializeObject(parameterValue, JsonSerializerSettings);
        }
    }
}
