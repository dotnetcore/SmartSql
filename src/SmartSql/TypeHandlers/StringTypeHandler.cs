using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class StringTypeHandler : AbstractNullableTypeHandler<String, String>
    {
        protected override string GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetString(columnIndex);
        }
    }
    public class StringAnyTypeHandler : AbstractNullableTypeHandler<String, AnyFieldType>
    {
        protected override string GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return dataReader.GetValue(columnIndex).ToString();
        }
    }
}
