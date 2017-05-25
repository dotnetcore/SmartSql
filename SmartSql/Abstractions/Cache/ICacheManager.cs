using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Abstractions.Cache
{
    public interface ICacheManager
    {
        ISmartSqlMapper SmartSqlMapper { get; set; }
        object this[RequestContext context, Type type] { get; set; }
        void ResetMappedCaches();
        void TriggerFlush(RequestContext context);
        Queue<RequestContext> RequestQueue { get; }
        void FlushQueue();
        void ClearQueue();
    }
}
