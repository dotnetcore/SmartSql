using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using SmartSql.DataSource;
using StatementType = SmartSql.Configuration.StatementType;

namespace SmartSql.InvokeSync
{
    public class SyncRequest
    {
        public Guid Id { get; set; }
        public Guid DbSessionId { get; set; }
        public DataSourceChoice DataSourceChoice { get; set; } = DataSourceChoice.Unknow;
        public CommandType CommandType { get; set; } = CommandType.Text;
        public StatementType? StatementType { get; set; }
        public IsolationLevel? Transaction { get; set; }
        public String ParameterPrefix { get; set; }
        public String ReadDb { get; set; }
        public bool IsStatementSql { get; internal set; } = true;
        public String RealSql { get; set; }
        /// <summary>
        /// SmartSqlMap.Scope
        /// </summary>
        public String Scope { get; set; }
        /// <summary>
        /// Statement.Id
        /// </summary>
        public String SqlId { get; set; }
        public IDictionary<string, object> Parameters { get; set; }
        public object Result { get; set; }
    }
}