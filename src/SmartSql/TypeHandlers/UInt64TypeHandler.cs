using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class UInt64TypeHandler : AbstractTypeHandler<UInt64, AnyFieldType>
    {
        public override UInt64 GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return Convert.ToUInt64(dataReader.GetValue(columnIndex));
        }
    }
}
