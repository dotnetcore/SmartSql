using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class ByteTypeHandler : AbstractTypeHandler<Byte>
    {
        public override Byte GetValue(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetByte(columnIndex);
        }
    }
}
