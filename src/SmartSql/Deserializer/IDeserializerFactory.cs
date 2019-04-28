using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Deserializer
{
    public interface IDeserializerFactory
    {
        IDataReaderDeserializer Get(ExecutionContext executionContext, Type resultType = null, bool isMultiple = false);
        void Add(IDataReaderDeserializer deserializer);
    }
}
