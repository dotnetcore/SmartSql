using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SmartSql.Data;
using SmartSql.TypeHandlers;

namespace SmartSql.TypeHandler
{
    public class XmlTypeHandler : AbstractNullableTypeHandler<Object, String>
    {
        public override object GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            if (dataReader.IsDBNull(columnIndex)) { return null; }
            var xmlStr = dataReader.GetString(columnIndex);
            return XmlSerializeUtil.Deserialize(xmlStr, targetType);
        }
        protected override object GetSetParameterValueWhenNotNull(object parameterValue)
        {
            return XmlSerializeUtil.Serializer(parameterValue);
        }
    }
}
