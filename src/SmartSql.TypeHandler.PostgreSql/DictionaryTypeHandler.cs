using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.TypeHandlers;

namespace SmartSql.TypeHandler.PostgreSql
{
    public class DictionaryTypeHandler : AbstractNullableTypeHandler<IDictionary<string, string>, AnyFieldType>
    {
    }
}
