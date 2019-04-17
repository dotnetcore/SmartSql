using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class NullableGuidTypeHandler : AbstractNullableTypeHandler<Guid?, Guid>
    {
        protected override Guid? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetGuid(columnIndex);
        }
    }
    public class NullableGuidAnyTypeHandler : AbstractNullableTypeHandler<Guid?, AnyFieldType>
    {
        protected override Guid? GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return new Guid(dataReader.GetValue(columnIndex).ToString());
        }
    }
}
