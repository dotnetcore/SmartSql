using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SmartSql.Data;

namespace SmartSql.TypeHandlers
{
    public class TimeSpanTypeHandler : AbstractTypeHandler<TimeSpan>
    {
        public override TimeSpan GetValue(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetFieldValue<TimeSpan>(columnIndex);
        }
    }
}
