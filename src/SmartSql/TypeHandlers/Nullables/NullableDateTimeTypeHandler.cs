using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SmartSql.Exceptions;

namespace SmartSql.TypeHandlers
{
    public class NullableDateTimeTypeHandler : AbstractNullableTypeHandler<DateTime?, DateTime>
    {
        protected override DateTime? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetDateTime(columnIndex);
        }
    }

    public class NullableDateTimeStringTypeHandler : AbstractNullableTypeHandler<DateTime?, String>
    {
        protected override DateTime? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            var strVal = dataReader.GetString(columnIndex);
            if (DateTime.TryParse(strVal, out var dateTime)) return dateTime;
            var colName = dataReader.GetName(columnIndex);
            throw new SmartSqlException($"Column.Name:{colName} String:[{strVal}] can not convert to DateTime.");
        }

        protected override object GetSetParameterValueWhenNotNull(object parameterValue)
        {
            return parameterValue.ToString();
        }
    }

    public class NullableDateTimeAnyTypeHandler : AbstractNullableTypeHandler<DateTime?, AnyFieldType>
    {
        protected override DateTime? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return Convert.ToDateTime(dataReader.GetValue(columnIndex));
        }
    }
}