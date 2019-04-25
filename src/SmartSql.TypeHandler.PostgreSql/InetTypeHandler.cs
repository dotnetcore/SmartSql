using SmartSql.TypeHandlers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SmartSql.TypeHandler.PostgreSql
{
    public class InetTypeHandler : AbstractNullableTypeHandler<ValueTuple<IPAddress, int>, AnyFieldType>
    {
    }
}
