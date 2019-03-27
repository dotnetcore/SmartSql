using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class UInt32TypeHandler : AbstractTypeHandler<UInt32, AnyFieldType>
    {
        public override UInt32 GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return Convert.ToUInt32(dataReader.GetValue(columnIndex));
        }
    }
}
