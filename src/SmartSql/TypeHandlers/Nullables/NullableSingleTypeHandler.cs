using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class NullableSingleTypeHandler : AbstractNullableTypeHandler<Single?, Single>
    {
        protected override Single? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetFloat(columnIndex);
        }
    }
    public class NullableSingleAnyTypeHandler : AbstractNullableTypeHandler<Single?, AnyFieldType>
    {
        protected override Single? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return Convert.ToSingle(dataReader.GetValue(columnIndex));
        }
    }
}
