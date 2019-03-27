using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class DecimalTypeHandler : AbstractTypeHandler<Decimal, Decimal>
    {
        public override Decimal GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return dataReader.GetDecimal(columnIndex);
        }
    }
    public class DecimalAnyTypeHandler : AbstractTypeHandler<Decimal, AnyFieldType>
    {
        public override Decimal GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return Convert.ToDecimal(dataReader.GetValue(columnIndex));
        }
    }
}
