using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class ByteTypeHandler : AbstractTypeHandler<Byte, Byte>
    {
        public override Byte GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return dataReader.GetByte(columnIndex);
        }
    }
    public class ByteAnyTypeHandler : AbstractTypeHandler<Byte, AnyFieldType>
    {
        public override Byte GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return Convert.ToByte(dataReader.GetValue(columnIndex));
        }
    }
}
