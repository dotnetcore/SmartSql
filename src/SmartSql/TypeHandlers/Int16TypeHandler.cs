using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SmartSql.Reflection.TypeConstants;

namespace SmartSql.TypeHandlers
{
    public class Int16TypeHandler : AbstractTypeHandler<Int16, Int16>
    {
        public override Int16 GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return dataReader.GetInt16(columnIndex);
        }
    }
    public class Int16ByteTypeHandler : AbstractTypeHandler<Int16, Byte>
    {
        public override Int16 GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return dataReader.GetByte(columnIndex);
        }

        public override object GetSetParameterValue(object parameterValue)
        {
            return Convert.ToByte(parameterValue);
        }
    }
    public class Int16AnyTypeHandler : AbstractTypeHandler<Int16, AnyFieldType>
    {
        public override Int16 GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return Convert.ToInt16(dataReader.GetValue(columnIndex));
        }
    }
}
