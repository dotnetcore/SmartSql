using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class GuidTypeHandler : AbstractTypeHandler<Guid>
    {
        public override Guid GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return dataReader.GetGuid(columnIndex);
        }
    }
}
