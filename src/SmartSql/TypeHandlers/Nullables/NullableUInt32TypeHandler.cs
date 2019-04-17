using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class NullableUInt32TypeHandler : AbstractNullableTypeHandler<UInt32?, AnyFieldType>
    {
        protected override UInt32? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetFieldValue<UInt32>(columnIndex);
        }
    }
}
