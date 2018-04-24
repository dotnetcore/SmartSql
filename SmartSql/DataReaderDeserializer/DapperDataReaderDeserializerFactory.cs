using SmartSql.Abstractions.DataReaderDeserializer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.DataReaderDeserializer
{
    public class DapperDataReaderDeserializerFactory : IDataReaderDeserializerFactory
    {
        public IDataReaderDeserializer Create(IDataReader dataReader, Type type)
        {
            throw new NotImplementedException();
        }
    }
}
