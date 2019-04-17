using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class NullableInt16TypeHandler : AbstractNullableTypeHandler<Int16?, Int16>
    {
        protected override Int16? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetInt16(columnIndex);
        }
    }
    public class NullableInt16AnyTypeHandler : AbstractNullableTypeHandler<Int16?, AnyFieldType>
    {
        protected override Int16? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return Convert.ToInt16(dataReader.GetValue(columnIndex));
        }
    }
}
