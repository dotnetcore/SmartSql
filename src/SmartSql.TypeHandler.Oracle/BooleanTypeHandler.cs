using SmartSql.Abstractions.TypeHandler;
using System;
using System.Data;

namespace SmartSql.TypeHandler.Oracle
{
    public class BooleanTypeHandler : ITypeHandler
    {
        public object GetSetParameterValue(object parameterValue)
        {
            if (parameterValue == null)
            {
                return 0;
            }
            else
            {
                return (bool)parameterValue ? 1 : 0;
            }
        }

        public object GetValue(IDataReader dataReader, string columnName, Type targetType)
        {
            int ordinal = dataReader.GetOrdinal(columnName);
            return GetValue(dataReader, ordinal, targetType);
        }

        public object GetValue(IDataReader dataReader, int columnIndex, Type targetType)
        {
            if (dataReader.IsDBNull(columnIndex)) { return false; }
            var value = dataReader.GetDecimal(columnIndex);
            return value > 0;
        }

        public virtual object SetParameter(IDataParameter dataParameter, object parameterValue)
        {
            dataParameter.Value = GetSetParameterValue(parameterValue);
            return dataParameter.Value;
        }
    }
}
