using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class NullableDecimalTypeHandler : AbstractNullableTypeHandler<Decimal?, Decimal>
    {
        protected override Decimal? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetDecimal(columnIndex);
        }
    }

    public class NullableDecimalAnyTypeHandler : AbstractNullableTypeHandler<Decimal?, AnyFieldType>
    {
        protected override Decimal? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return Convert.ToDecimal(dataReader.GetValue(columnIndex));
        }
    }
}
