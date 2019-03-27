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
    public class Int64AnyTypeHandler : AbstractTypeHandler<Int64, AnyFieldType>
    {
        public override long GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return Convert.ToInt64(dataReader.GetValue(columnIndex));
        }
    }
}
