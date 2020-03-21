using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class NullableUInt64TypeHandler : AbstractNullableTypeHandler<UInt64?, UInt64>
    {
        protected override UInt64? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return Convert.ToUInt64(dataReader.GetValue(columnIndex));
        }
    }
    public class NullableUInt64AnyTypeHandler : AbstractNullableTypeHandler<UInt64?, AnyFieldType>
    {
        protected override UInt64? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return Convert.ToUInt64(dataReader.GetValue(columnIndex));
        }
    }
}
