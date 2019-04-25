using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SmartSql.TypeHandlers;

namespace SmartSql.TypeHandler.PostgreSql
{
    public class Int64ArrayTypeHandler : AbstractNullableTypeHandler<Int64[], AnyFieldType>
    {
    }
}
