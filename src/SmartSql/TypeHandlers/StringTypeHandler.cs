using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class StringTypeHandler : AbstractNullableTypeHandler<String>
    {
        protected override string GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetString(columnIndex);
        }
    }
}
