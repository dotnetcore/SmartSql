using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class ByteArrayTypeHandler : AbstractTypeHandler<Byte[], Byte[]>
    {
        public override byte[] GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            int bufferLength = (int)dataReader.GetBytes(columnIndex, 0, null, 0, 0);
            byte[] byteArray = new byte[bufferLength];
            dataReader.GetBytes(columnIndex, 0, byteArray, 0, bufferLength);
            return byteArray;
        }
    }
    public class ByteArrayAnyTypeHandler : AbstractTypeHandler<Byte[], AnyFieldType>
    {
        public override byte[] GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            int bufferLength = (int)dataReader.GetBytes(columnIndex, 0, null, 0, 0);
            byte[] byteArray = new byte[bufferLength];
            dataReader.GetBytes(columnIndex, 0, byteArray, 0, bufferLength);
            return byteArray;
        }
    }
}
