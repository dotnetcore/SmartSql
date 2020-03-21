using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class NullableUInt16TypeHandler : AbstractNullableTypeHandler<UInt16?, UInt16>
    {
        protected override UInt16? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return Convert.ToUInt16(dataReader.GetValue(columnIndex));
        }
    }
    public class NullableUInt16AnyTypeHandler : AbstractNullableTypeHandler<UInt16?, AnyFieldType>
    {
        protected override UInt16? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return Convert.ToUInt16(dataReader.GetValue(columnIndex));
        }
    }
}
