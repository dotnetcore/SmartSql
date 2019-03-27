using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class NullableDoubleTypeHandler : AbstractNullableTypeHandler<Double?, Double>
    {
        protected override Double? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetDouble(columnIndex);
        }
    }
    public class NullableDoubleAnyTypeHandler : AbstractNullableTypeHandler<Double?, AnyFieldType>
    {
        protected override Double? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return Convert.ToDouble(dataReader.GetValue(columnIndex));
        }
    }
}
