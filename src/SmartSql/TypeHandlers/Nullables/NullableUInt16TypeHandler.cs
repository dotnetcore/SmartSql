using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class NullableUInt16TypeHandler : AbstractNullableTypeHandler<UInt16?>
    {
        protected override UInt16? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetFieldValue<UInt16>(columnIndex);
        }
    }
}
