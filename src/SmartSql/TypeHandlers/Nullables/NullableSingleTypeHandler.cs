using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class NullableSingleTypeHandler : AbstractNullableTypeHandler<Single?>
    {
        protected override Single? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetFloat(columnIndex);
        }
    }
}
