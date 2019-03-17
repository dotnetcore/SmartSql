using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class NullableInt16TypeHandler : AbstractNullableTypeHandler<Int16?>
    {
        protected override Int16? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetInt16(columnIndex);
        }

    }
}
