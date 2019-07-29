using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SmartSql.Exceptions;

namespace SmartSql.TypeHandlers
{
    public class NullableBooleanTypeHandler : AbstractNullableTypeHandler<bool?, bool>
    {
        protected override bool? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetBoolean(columnIndex);
        }
    }

    public class NullableBooleanCharTypeHandler : AbstractNullableTypeHandler<bool?, Char>
    {
        protected override bool? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            var charVal = dataReader.GetChar(columnIndex);
            if (charVal == BooleanCharTypeHandler.TRUE || charVal == BooleanCharTypeHandler.FALSE)
            {
                return charVal == BooleanCharTypeHandler.TRUE;
            }

            throw new SmartSqlException($"NullableBooleanCharTypeHandler Can not Convert :[{charVal}] To Boolean.");
        }

        protected override object GetSetParameterValueWhenNotNull(object parameterValue)
        {
            var propertyVal = (bool) parameterValue;
            if (propertyVal)
            {
                return BooleanCharTypeHandler.TRUE;
            }

            return BooleanCharTypeHandler.FALSE;
        }
    }

    public class NullableBooleanStringTypeHandler : AbstractNullableTypeHandler<bool?, String>
    {
        protected override bool? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            var strVal = dataReader.GetString(columnIndex);
            if (strVal == BooleanStringTypeHandler.TRUE || strVal == BooleanStringTypeHandler.FALSE)
            {
                return strVal == BooleanStringTypeHandler.TRUE;
            }

            if (Boolean.TryParse(strVal, out bool valResult)) return valResult;
            throw new SmartSqlException($"NullableBooleanStringTypeHandler Can not Convert :[{strVal}] To Boolean.");
        }

        protected override object GetSetParameterValueWhenNotNull(object parameterValue)
        {
            var propertyVal = (bool) parameterValue;
            if (propertyVal)
            {
                return BooleanStringTypeHandler.TRUE;
            }

            return BooleanStringTypeHandler.FALSE;
        }
    }

    public class NullableBooleanAnyTypeHandler : AbstractNullableTypeHandler<bool?, AnyFieldType>
    {
        protected override bool? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return Convert.ToBoolean(dataReader.GetValue(columnIndex));
        }
    }
}