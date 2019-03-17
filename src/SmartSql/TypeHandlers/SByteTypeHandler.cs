using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class SByteTypeHandler : AbstractNullableTypeHandler<SByte>
    {
        public override SByte GetValue(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetFieldValue<SByte>(columnIndex);
        }
    }
}
