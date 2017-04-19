using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Abstractions.Cache
{
    public interface ICacheManager
    {
        object this[RequestContext context, Type type] { get; set; }
        void TriggerFlush(RequestContext context);
    }
}
