using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class NullableInt32TypeHandler : AbstractNullableTypeHandler<Int32?, Int32>
    {
        protected override Int32? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetInt32(columnIndex);
        }
    }
    public class NullableInt32AnyTypeHandler : AbstractNullableTypeHandler<Int32?, AnyFieldType>
    {
        protected override Int32? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return Convert.ToInt32(dataReader.GetValue(columnIndex));
        }
    }
}
