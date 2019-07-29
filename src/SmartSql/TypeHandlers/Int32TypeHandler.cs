using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class Int32TypeHandler : AbstractTypeHandler<Int32, Int32>
    {
        public override Int32 GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return dataReader.GetInt32(columnIndex);
        }
    }
    public class Int32ByteTypeHandler : AbstractTypeHandler<Int32, Byte>
    {
        public override Int32 GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return dataReader.GetByte(columnIndex);
        }

        public override object GetSetParameterValue(object parameterValue)
        {
            return Convert.ToByte(parameterValue);
        }
    }
    public class Int32Int16TypeHandler : AbstractTypeHandler<Int32, Int16>
    {
        public override Int32 GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return dataReader.GetInt16(columnIndex);
        }
        public override object GetSetParameterValue(object parameterValue)
        {
            return Convert.ToInt16(parameterValue);
        }
    }
    public class Int32Int64TypeHandler : AbstractTypeHandler<Int32, Int64>
    {
        public override Int32 GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return (int)dataReader.GetInt64(columnIndex);
        }
        public override object GetSetParameterValue(object parameterValue)
        {
            return Convert.ToInt64(parameterValue);
        }
    }
    public class Int32AnyTypeHandler : AbstractTypeHandler<Int32, AnyFieldType>
    {
        public override Int32 GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return Convert.ToInt32(dataReader.GetValue(columnIndex));
        }
    }
}
