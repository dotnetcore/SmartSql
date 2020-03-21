using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class UInt64TypeHandler : AbstractTypeHandler<UInt64, UInt64>
    {
        public override UInt64 GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return Convert.ToUInt64(dataReader.GetValue(columnIndex));
        }
    }

    public class UInt64ByteTypeHandler : AbstractTypeHandler<UInt64, Byte>
    {
        public override UInt64 GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return dataReader.GetByte(columnIndex);
        }

        public override object GetSetParameterValue(object parameterValue)
        {
            return Convert.ToByte(parameterValue);
        }
    }

    public class UInt64UInt16TypeHandler : AbstractTypeHandler<UInt64, UInt16>
    {
        public override UInt64 GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return Convert.ToUInt64(dataReader.GetInt16(columnIndex));
        }

        public override object GetSetParameterValue(object parameterValue)
        {
            return Convert.ToUInt16(parameterValue);
        }
    }

    public class UInt64UInt32TypeHandler : AbstractTypeHandler<UInt64, UInt32>
    {
        public override UInt64 GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return Convert.ToUInt64(dataReader.GetInt32(columnIndex));
        }

        public override object GetSetParameterValue(object parameterValue)
        {
            return Convert.ToUInt32(parameterValue);
        }
    }

    public class UInt64AnyTypeHandler : AbstractTypeHandler<UInt64, AnyFieldType>
    {
        public override UInt64 GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return Convert.ToUInt64(dataReader.GetValue(columnIndex));
        }
    }
}