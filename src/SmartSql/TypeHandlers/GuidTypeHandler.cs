using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class GuidTypeHandler : AbstractTypeHandler<Guid, Guid>
    {
        public override Guid GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return dataReader.GetGuid(columnIndex);
        }
    }
    public class GuidAnyTypeHandler : AbstractTypeHandler<Guid, AnyFieldType>
    {
        public override Guid GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return new Guid(dataReader.GetValue(columnIndex).ToString());
        }
    }
}
