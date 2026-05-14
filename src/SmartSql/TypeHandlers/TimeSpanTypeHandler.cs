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
            var val = dataReader.GetValue(columnIndex);
            if (val is TimeSpan ts) return ts;
            if (val is Int64 ticks) return new TimeSpan(ticks);
            if (val is string str) return TimeSpan.Parse(str);
            return new TimeSpan(Convert.ToInt64(val));
        }
    }
}
