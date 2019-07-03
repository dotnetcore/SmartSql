using SmartSql.Configuration;
using SmartSql.DbSession;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace SmartSql.Cache
{
    public class CacheManager : AbstractCacheManager
    {
        protected override void ListenInvokeSucceeded()
        {
            SmartSqlConfig.InvokeSucceedListener.InvokeSucceeded += (sender, args) =>
            {
                var reqContext = args.ExecutionContext.Request;
                if (reqContext.IsStatementSql)
                {
                    FlushOnExecuted(reqContext.FullSqlId);
                }
            };
        }
    }
}