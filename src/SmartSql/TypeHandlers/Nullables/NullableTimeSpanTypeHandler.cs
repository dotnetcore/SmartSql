using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SmartSql.Data;

namespace SmartSql.TypeHandlers
{
    public class NullableTimeSpanTypeHandler : AbstractNullableTypeHandler<TimeSpan?, TimeSpan>
    {
        protected override TimeSpan? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetFieldValue<TimeSpan>(columnIndex);
        }
    }
    public class NullableTimeSpanAnyTypeHandler : AbstractNullableTypeHandler<TimeSpan?, AnyFieldType>
    {
        protected override TimeSpan? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return new TimeSpan(Convert.ToInt64(dataReader.GetValue(columnIndex)));
        }
    }
}
