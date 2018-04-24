using SmartSql.Abstractions.DataReaderDeserializer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.DataReaderDeserializer
{
    public class DapperDataReaderDeserializer : IDataReaderDeserializer
    {
        public object Deserialize(IDataReader dataReader, Type type)
        {
            throw new NotImplementedException();
        }
    }
}
