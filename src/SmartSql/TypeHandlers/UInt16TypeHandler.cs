using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class UInt16TypeHandler : AbstractTypeHandler<UInt16, AnyFieldType>
    {
        public override UInt16 GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return Convert.ToUInt16(dataReader.GetValue(columnIndex));
        }
    }
}
