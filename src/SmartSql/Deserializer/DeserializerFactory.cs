using SmartSql.Reflection.TypeConstants;
using System;

namespace SmartSql.Deserializer
{
    public class DeserializerFactory : IDeserializerFactory
    {
        private readonly IDataReaderDeserializer _defaultDeser;
        private readonly IDataReaderDeserializer _valueTypeDeser;
        private readonly IDataReaderDeserializer _dynamicDeser;
        private readonly IDataReaderDeserializer _multipleDeser;

        public DeserializerFactory()
        {
            _valueTypeDeser = new ValueTypeDeserializer();
            _defaultDeser = new EntityDeserializer();
            _dynamicDeser = new DynamicDeserializer();
            _multipleDeser = new MultipleResultDeserializer(this);
        }

        public IDataReaderDeserializer Get(ExecutionContext executionContext, Type resultType = null)
        {
            if (executionContext.Request.MultipleResultMap != null)
            {
                return _multipleDeser;
            }
            if (resultType == null)
            {
                resultType = executionContext.Result.ResultType;
            }

            return Get(resultType);
        }

        public IDataReaderDeserializer Get(Type resultType)
        {
            if (resultType.IsValueType || resultType == CommonType.String)
            {
                return _valueTypeDeser;
            }
            return resultType == CommonType.Object ? _dynamicDeser : _defaultDeser;
        }
    }
}
