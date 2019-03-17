using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class NullableGuidTypeHandler : AbstractNullableTypeHandler<Guid?>
    {
        protected override Guid? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetGuid(columnIndex);
        }
    }
}
