using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class CharTypeHandler : AbstractTypeHandler<Char, String>
    {
        public override Char GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return dataReader.GetString(columnIndex)[0];
        }

        public override object GetSetParameterValue(object parameterValue)
        {
            return Convert.ToString(parameterValue);
        }
    }

    public class CharAnyTypeHandler : AbstractTypeHandler<Char, AnyFieldType>
    {
        public override Char GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return dataReader.GetString(columnIndex)[0];
        }
    }
}