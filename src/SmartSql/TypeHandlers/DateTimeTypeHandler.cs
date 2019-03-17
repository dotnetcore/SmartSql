using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class DateTimeTypeHandler : AbstractTypeHandler<DateTime>
    {
        public override DateTime GetValue(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetDateTime(columnIndex);
        }
    }
}
