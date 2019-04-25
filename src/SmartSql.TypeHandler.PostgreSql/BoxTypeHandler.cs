using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using NpgsqlTypes;
using SmartSql.TypeHandlers;

namespace SmartSql.TypeHandler.PostgreSql
{
    public class BoxTypeHandler : AbstractNullableTypeHandler<NpgsqlBox, NpgsqlBox>
    {
    }
}
