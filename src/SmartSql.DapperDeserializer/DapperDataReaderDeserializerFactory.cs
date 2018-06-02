using SmartSql.Abstractions.DataReaderDeserializer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.DapperDeserializer
{
    public class DapperDataReaderDeserializerFactory : IDataReaderDeserializerFactory
    {
        private IDataReaderDeserializer _dataReaderDeserializer;

        public IDataReaderDeserializer Create()
        {
            if (_dataReaderDeserializer == null)
            {
                lock (this)
                {
                    if (_dataReaderDeserializer == null)
                    {
                        _dataReaderDeserializer = new DapperDataReaderDeserializer();
                    }
                }
            }
            return _dataReaderDeserializer;
        }
    }
}
