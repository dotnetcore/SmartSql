using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class DecimalTypeHandler : AbstractTypeHandler<Decimal>
    {
        public override Decimal GetValue(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetDecimal(columnIndex);
        }
    }
}
