using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.Abstractions.TypeHandler
{
    public abstract class BaseTypeHandler : ITypeHandler
    {
        public virtual object GetValue(IDataReader dataReader, string columnName, Type targetType)
        {
            int ordinal = dataReader.GetOrdinal(columnName);
            return GetValue(dataReader, ordinal, targetType);
        }

        public virtual object GetValue(IDataReader dataReader, int columnIndex, Type targetType)
        {
            var val = dataReader.GetValue(columnIndex);
            if (val is DBNull)
            {
                return null;
            }
            return val;
        }
        public object GetSetParameterValue(object parameterValue)
        {
            if (parameterValue == null)
            {
                return DBNull.Value;
            }
            else
            {
                return parameterValue;
            }
        }
        public object SetParameter(IDataParameter dataParameter, object parameterValue)
        {
            dataParameter.Value = GetSetParameterValue(parameterValue);
            return dataParameter.Value;
        }
    }
}
