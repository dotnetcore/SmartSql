using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class BooleanTypeHandler : AbstractTypeHandler<Boolean, Boolean>
    {
        public override Boolean GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return dataReader.GetBoolean(columnIndex);
        }
    }
    public class BooleanAnyTypeHandler : AbstractTypeHandler<Boolean, AnyFieldType>
    {
        public override Boolean GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return Convert.ToBoolean(dataReader.GetValue(columnIndex));
        }
    }
}
