using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SmartSql.Data;

namespace SmartSql.TypeHandlers
{
    public class TimeSpanTypeHandler : AbstractTypeHandler<TimeSpan, TimeSpan>
    {
        public override TimeSpan GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return dataReader.GetFieldValue<TimeSpan>(columnIndex);
        }
    }
    public class TimeSpanAnyTypeHandler : AbstractTypeHandler<TimeSpan, AnyFieldType>
    {
        public override TimeSpan GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return new TimeSpan(Convert.ToInt64(dataReader.GetValue(columnIndex)));
        }
    }
}
