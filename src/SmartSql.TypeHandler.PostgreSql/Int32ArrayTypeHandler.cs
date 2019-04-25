using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SmartSql.TypeHandlers;

namespace SmartSql.TypeHandler.PostgreSql
{
    public class Int32ArrayTypeHandler : AbstractNullableTypeHandler<Int32[], AnyFieldType>
    {
    }
}
