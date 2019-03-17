using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class Int32TypeHandler : AbstractTypeHandler<Int32>
    {
        public override Int32 GetValue(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetInt32(columnIndex);
        }
    }
}
