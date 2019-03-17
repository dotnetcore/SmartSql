using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class UInt16TypeHandler : AbstractTypeHandler<UInt16>
    {
        public override UInt16 GetValue(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetFieldValue<UInt16>(columnIndex);
        }
    }
}
