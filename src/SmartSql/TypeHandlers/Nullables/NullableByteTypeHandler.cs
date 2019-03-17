using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class NullableByteTypeHandler : AbstractNullableTypeHandler<Byte?>
    {
        protected override Byte? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetByte(columnIndex);
        }
    }
}
