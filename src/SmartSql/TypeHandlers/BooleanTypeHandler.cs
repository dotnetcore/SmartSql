using SmartSql.Data;
using System;
using SmartSql.Exceptions;

namespace SmartSql.TypeHandlers
{
    public class BooleanTypeHandler : AbstractTypeHandler<Boolean, Boolean>
    {
        public override Boolean GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return dataReader.GetBoolean(columnIndex);
        }
    }

    public class BooleanCharTypeHandler : AbstractTypeHandler<Boolean, Char>
    {
        public const char TRUE = '1';
        public const char FALSE = '0';

        public override Boolean GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            var charVal = dataReader.GetChar(columnIndex);
            if (charVal == TRUE || charVal == FALSE)
            {
                return charVal == TRUE;
            }

            throw new SmartSqlException($"BooleanCharTypeHandler Can not Convert :[{charVal}] To Boolean.");
        }

        public override object GetSetParameterValue(object parameterValue)
        {
            var propertyVal = (bool) parameterValue;
            if (propertyVal)
            {
                return TRUE;
            }
            return FALSE;
        }
    }

    public class BooleanStringTypeHandler : AbstractTypeHandler<Boolean, String>
    {
        public const string TRUE = "1";
        public const string FALSE = "0";

        public override Boolean GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            var strVal = dataReader.GetString(columnIndex);
            if (strVal == TRUE || strVal == FALSE)
            {
                return strVal == TRUE;
            }

            if (Boolean.TryParse(strVal, out bool valResult)) return valResult;
            throw new SmartSqlException($"BooleanStringTypeHandler Can not Convert :[{strVal}] To Boolean.");
        }
        public override object GetSetParameterValue(object parameterValue)
        {
            var propertyVal = (bool) parameterValue;
            if (propertyVal)
            {
                return TRUE;
            }
            return FALSE;
        }
    }

    public class BooleanAnyTypeHandler : AbstractTypeHandler<Boolean, AnyFieldType>
    {
        public override Boolean GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return Convert.ToBoolean(dataReader.GetValue(columnIndex));
        }
    }
}