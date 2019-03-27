using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class SingleTypeHandler : AbstractNullableTypeHandler<Single, Single>
    {
        public override Single GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return dataReader.GetFloat(columnIndex);
        }
    }
    public class SingleAnyTypeHandler : AbstractNullableTypeHandler<Single, AnyFieldType>
    {
        public override Single GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return Convert.ToSingle(dataReader.GetValue(columnIndex));
        }
    }
}
