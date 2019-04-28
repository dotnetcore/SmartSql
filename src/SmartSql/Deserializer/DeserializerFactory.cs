using SmartSql.Reflection.TypeConstants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartSql.Deserializer
{
    public class DeserializerFactory : IDeserializerFactory
    {
        private readonly IList<IDataReaderDeserializer> _deserCache = new List<IDataReaderDeserializer>();
        private readonly IDataReaderDeserializer _Default_Deserializer;
        public DeserializerFactory()
        {
            _Default_Deserializer = new EntityDeserializer();
        }

        public IDataReaderDeserializer Get(ExecutionContext executionContext, Type resultType = null, bool isMultiple = false)
        {
            resultType = resultType ?? executionContext.Result.ResultType;

            var deser = _deserCache.FirstOrDefault(d => d.CanDeserialize(executionContext, resultType, isMultiple));
            return deser ?? _Default_Deserializer;
        }

        public void Add(IDataReaderDeserializer deserializer)
        {
            _deserCache.Add(deserializer);
        }
    }
}
