using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class UInt64TypeHandler : AbstractTypeHandler<UInt64>
    {
        public override UInt64 GetValue(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetFieldValue<UInt64>(columnIndex);
        }
    }
}
