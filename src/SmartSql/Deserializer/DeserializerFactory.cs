using SmartSql.Reflection.TypeConstants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartSql.Deserializer
{
    public class DeserializerFactory : IDeserializerFactory
    {
        private readonly IList<IDataReaderDeserializer> _deserCache = new List<IDataReaderDeserializer>();

        public IDataReaderDeserializer Get(ExecutionContext executionContext, Type resultType = null, bool isMultiple = false)
        {
            resultType = resultType ?? executionContext.Result.ResultType;

            return _deserCache.FirstOrDefault(d => d.CanDeserialize(executionContext, resultType, isMultiple));
        }

        public void Add(IDataReaderDeserializer deserializer)
        {
            _deserCache.Add(deserializer);
        }
    }
}
