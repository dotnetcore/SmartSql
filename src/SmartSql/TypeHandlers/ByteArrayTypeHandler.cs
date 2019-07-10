using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class ByteArrayTypeHandler : AbstractNullableTypeHandler<Byte[], Byte[]>
    {
        protected override byte[] GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            var bufferLength = (int) dataReader.GetBytes(columnIndex, 0, null, 0, 0);
            byte[] byteArray = new byte[bufferLength];
            if (bufferLength == 0)
            {
                return byteArray;
            }
            dataReader.GetBytes(columnIndex, 0, byteArray, 0, bufferLength);
            return byteArray;
        }
    }

    public class ByteArrayAnyTypeHandler : AbstractNullableTypeHandler<Byte[], AnyFieldType>
    {
        protected override byte[] GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            var fieldValue = dataReader.GetValue(columnIndex).ToString();
            return Encoding.Default.GetBytes(fieldValue);
        }
    }
}