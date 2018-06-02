using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.Abstractions.DataReaderDeserializer
{
    public interface IDataReaderDeserializerFactory
    {
        IDataReaderDeserializer Create();
    }
}
