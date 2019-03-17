using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class NullableBooleanTypeHandler : AbstractNullableTypeHandler<bool?>
    {
        protected override bool? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetBoolean(columnIndex);
        }
    }
}
