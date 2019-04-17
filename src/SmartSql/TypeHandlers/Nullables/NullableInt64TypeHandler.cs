using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class NullableInt64TypeHandler : AbstractNullableTypeHandler<Int64?, Int64>
    {
        protected override Int64? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetInt64(columnIndex);
        }
    }
    public class NullableInt64AnyTypeHandler : AbstractNullableTypeHandler<Int64?, AnyFieldType>
    {
        protected override Int64? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return Convert.ToInt64(dataReader.GetValue(columnIndex));
        }
    }
}
