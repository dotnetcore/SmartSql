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
    public class Int32AnyTypeHandler : AbstractTypeHandler<Int32, AnyFieldType>
    {
        public override Int32 GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return Convert.ToInt32(dataReader.GetValue(columnIndex));
        }
    }
}
