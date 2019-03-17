using System;
using System.Collections.Generic;
using System.Data;

namespace SmartSql.TypeHandler.PostgreSql
{
    public class JsonTypeHandler<T> : TypeHandler.JsonTypeHandler<T>
    {
        private const string DATA_TYPE_NAME = "DataTypeName";
        private string _dataTypeName = "json";
        public override void Initialize(IDictionary<string, object> parameters)
        {
            base.Initialize(parameters);
            if (parameters.Value<string, object, string>(DATA_TYPE_NAME, out var dataTypeName))
            {
                _dataTypeName = dataTypeName;
            }
        }
        public override void SetParameter(IDataParameter dataParameter, object parameterValue)
        {
            base.SetParameter(dataParameter, parameterValue);
            var npgParam = dataParameter as Npgsql.NpgsqlParameter;
            if (!String.IsNullOrEmpty(_dataTypeName))
            {
                npgParam.DataTypeName = _dataTypeName;
            }
        }
    }
}
