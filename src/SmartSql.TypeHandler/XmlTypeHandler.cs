using SmartSql.Abstractions.TypeHandler;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace SmartSql.TypeHandler
{
    public class XmlTypeHandler : ITypeHandler
    {


        public object GetValue(IDataReader dataReader, string columnName, Type targetType)
        {
            int ordinal = dataReader.GetOrdinal(columnName);
            return GetValue(dataReader, ordinal, targetType);
        }

        public object GetValue(IDataReader dataReader, int columnIndex, Type targetType)
        {
            if (dataReader.IsDBNull(columnIndex)) { return null; }
            var xmlStr = dataReader.GetString(columnIndex);
            return XmlSerializeUtil.Deserialize(xmlStr, targetType);
        }
        public object GetSetParameterValue(object parameterValue)
        {
            if (parameterValue == null)
            {
                return DBNull.Value;
            }
            else
            {
                return XmlSerializeUtil.Serializer(parameterValue);
            }
        }
        public object SetParameter(IDataParameter dataParameter, object parameterValue)
        {
            dataParameter.Value = GetSetParameterValue(parameterValue);
            return dataParameter.Value;
        }
    }
}
