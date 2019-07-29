using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class NullableCharTypeHandler : AbstractNullableTypeHandler<Char?, String>
    {
        protected override Char? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetString(columnIndex)[0];
        }

        protected override object GetSetParameterValueWhenNotNull(object parameterValue)
        {
            return parameterValue.ToString();
        }
    }

    public class NullableCharAnyTypeHandler : AbstractNullableTypeHandler<Char?, AnyFieldType>
    {
        protected override Char? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return Convert.ToChar(dataReader.GetValue(columnIndex));
        }
    }
}