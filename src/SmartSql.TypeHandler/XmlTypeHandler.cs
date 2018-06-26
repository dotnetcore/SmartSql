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

        public void SetParameter(IDataParameter dataParameter, object parameterValue)
        {
            if (parameterValue == null)
            {
                dataParameter.Value = DBNull.Value;
            }
            else
            {
                var xmlStr = XmlSerializeUtil.Serializer(parameterValue);
                dataParameter.Value = xmlStr;
            }
        }
    }
}
