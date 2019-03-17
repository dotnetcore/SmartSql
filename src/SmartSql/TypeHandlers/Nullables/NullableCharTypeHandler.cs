using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class NullableCharTypeHandler : AbstractNullableTypeHandler<Char?>
    {
        protected override Char? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetString(columnIndex)[0];
        }
    }
}
