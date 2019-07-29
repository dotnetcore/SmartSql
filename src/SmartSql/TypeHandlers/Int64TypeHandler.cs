using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class Int64TypeHandler : AbstractTypeHandler<Int64, Int64>
    {
        public override long GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return dataReader.GetInt64(columnIndex);
        }
    }
    public class Int64ByteTypeHandler : AbstractTypeHandler<Int64, Byte>
    {
        public override long GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return dataReader.GetByte(columnIndex);
        }
        public override object GetSetParameterValue(object parameterValue)
        {
            return Convert.ToByte(parameterValue);
        }
    }
    public class Int64Int16TypeHandler : AbstractTypeHandler<Int64, Int16>
    {
        public override long GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return dataReader.GetInt16(columnIndex);
        }
        public override object GetSetParameterValue(object parameterValue)
        {
            return Convert.ToInt16(parameterValue);
        }
    }
    public class Int64Int32TypeHandler : AbstractTypeHandler<Int64, Int32>
    {
        public override long GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return dataReader.GetInt32(columnIndex);
        }
        public override object GetSetParameterValue(object parameterValue)
        {
            return Convert.ToInt32(parameterValue);
        }
    }

    public class Int64AnyTypeHandler : AbstractTypeHandler<Int64, AnyFieldType>
    {
        public override long GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return Convert.ToInt64(dataReader.GetValue(columnIndex));
        }
    }
}
