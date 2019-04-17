using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class NullableByteTypeHandler : AbstractNullableTypeHandler<Byte?, Byte>
    {
        protected override Byte? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetByte(columnIndex);
        }
    }
    public class NullableByteAnyTypeHandler : AbstractNullableTypeHandler<Byte?, AnyFieldType>
    {
        protected override Byte? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return Convert.ToByte(dataReader.GetValue(columnIndex));
        }
    }
}
