using System;
using System.Collections.Generic;
using SmartSql.Configuration;

namespace SmartSql.InvokeSync
{
    public class SyncFilterOptions
    {
        public StatementType StatementType { get; set; } = StatementType.Write;
        public IEnumerable<String> Scopes { get; set; }
        public IEnumerable<String> SqlIds { get; set; }
        public IEnumerable<String> FullSqlIds { get; set; }
        public StatementType? IgnoreStatementType { get; set; }
        public IEnumerable<String> IgnoreScopes { get; set; }
        public IEnumerable<String> IgnoreSqlIds { get; set; }
        public IEnumerable<String> IgnoreFullSqlIds { get; set; }
    }
}