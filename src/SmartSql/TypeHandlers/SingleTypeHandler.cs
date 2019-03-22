using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class SingleTypeHandler : AbstractNullableTypeHandler<Single>
    {
        public override Single GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return dataReader.GetFloat(columnIndex);
        }
    }
}
