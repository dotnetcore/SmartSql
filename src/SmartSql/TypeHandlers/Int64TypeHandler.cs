using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class Int64TypeHandler : AbstractTypeHandler<Int64>
    {
        public override long GetValue(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetInt64(columnIndex);
        }
    }
}
