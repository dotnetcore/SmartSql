using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class BooleanTypeHandler : AbstractTypeHandler<Boolean>
    {
        public override Boolean GetValue(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetBoolean(columnIndex);
        }
    }
}
