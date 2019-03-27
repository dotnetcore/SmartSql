using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class NullableBooleanTypeHandler : AbstractNullableTypeHandler<bool?,bool>
    {
        protected override bool? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetBoolean(columnIndex);
        }
    }
    public class NullableBooleanAnyTypeHandler : AbstractNullableTypeHandler<bool?, AnyFieldType>
    {
        protected override bool? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return Convert.ToBoolean(dataReader.GetValue(columnIndex));
        }
    }
}
