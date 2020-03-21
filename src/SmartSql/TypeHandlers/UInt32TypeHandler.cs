using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class UInt32TypeHandler : AbstractTypeHandler<UInt32, UInt32>
    {
        public override UInt32 GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return Convert.ToUInt32(dataReader.GetValue(columnIndex));
        }
    }
    
    public class UInt32ByteTypeHandler : AbstractTypeHandler<UInt32, Byte>
    {
        public override UInt32 GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return dataReader.GetByte(columnIndex);
        }

        public override object GetSetParameterValue(object parameterValue)
        {
            return Convert.ToByte(parameterValue);
        }
    }
    public class UInt32UInt16TypeHandler : AbstractTypeHandler<UInt32, UInt16>
    {
        public override UInt32 GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return Convert.ToUInt32(dataReader.GetInt16(columnIndex));
        }
        public override object GetSetParameterValue(object parameterValue)
        {
            return Convert.ToUInt16(parameterValue);
        }
    }
    public class UInt32AnyTypeHandler : AbstractTypeHandler<UInt32, AnyFieldType>
    {
        public override UInt32 GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return Convert.ToUInt32(dataReader.GetValue(columnIndex));
        }
    }
}
