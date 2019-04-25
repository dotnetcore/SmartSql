using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SmartSql.TypeHandlers;

namespace SmartSql.TypeHandler.PostgreSql
{
    public class StringArrayTypeHandler : AbstractNullableTypeHandler<String[], AnyFieldType>
    {
    }
}
