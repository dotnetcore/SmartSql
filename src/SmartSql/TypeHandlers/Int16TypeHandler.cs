using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class Int16TypeHandler : AbstractTypeHandler<Int16>
    {
        public override Int16 GetValue(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetInt16(columnIndex);
        }
    }
}
