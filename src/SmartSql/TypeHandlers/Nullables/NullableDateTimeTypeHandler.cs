using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class NullableDateTimeTypeHandler : AbstractNullableTypeHandler<DateTime?>
    {
        protected override DateTime? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetDateTime(columnIndex);
        }
    }
}
