using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SmartSql.Exceptions;

namespace SmartSql.TypeHandlers
{
    public class DateTimeTypeHandler : AbstractTypeHandler<DateTime, DateTime>
    {
        public override DateTime GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return dataReader.GetDateTime(columnIndex);
        }
    }

    public class DateTimeStringTypeHandler : AbstractTypeHandler<DateTime, String>
    {
        public override DateTime GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            var strVal = dataReader.GetString(columnIndex);
            if (DateTime.TryParse(strVal, out var dateTime)) return dateTime;
            var colName = dataReader.GetName(columnIndex);
            throw new SmartSqlException($"Column.Name:{colName} String:[{strVal}] can not convert to DateTime.");
        }

        public override object GetSetParameterValue(object parameterValue)
        {
            return parameterValue.ToString();
        }
    }
    public class DateTimeAnyTypeHandler : AbstractTypeHandler<DateTime, AnyFieldType>
    {
        public override DateTime GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return Convert.ToDateTime(dataReader.GetValue(columnIndex));
        }
    }
}
