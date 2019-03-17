using SmartSql.Configuration;
using SmartSql.DataSource;
using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using SmartSql.Data;

namespace SmartSql
{
    public class RequestContext
    {
        public ExecutionContext ExecutionContext { get; internal set; }
        public ExecutionType ExecutionType { get; set; }
        public DataSourceChoice DataSourceChoice { get; set; } = DataSourceChoice.Unknow;
        public CommandType CommandType { get; set; } = CommandType.Text;
        public String ReadDb { get; set; }
        public Statement Statement { get; internal set; }
        public StringBuilder SqlBuilder { get; internal set; }
        public bool IsStatementSql { get; internal set; } = true;
        internal bool IgnorePrepend { get; set; } = false;
        public String RealSql { get; set; }
        /// <summary>
        /// SmartSqlMap.Scope
        /// </summary>
        public String Scope { get; set; }
        /// <summary>
        /// Statement.Id
        /// </summary>
        public String SqlId { get; set; }
        /// <summary>
        /// Statement.FullSqlId
        /// </summary>
        public String FullSqlId => $"{Scope}.{SqlId}";
        #region Map
        public String CacheId { get; set; }
        public Configuration.Cache Cache { get; internal set; }
        public string ResultMapId { get; set; }
        public ResultMap ResultMap { get; internal set; }
        public string MultipleResultMapId { get; set; }
        public MultipleResultMap MultipleResultMap { get; internal set; }
        #endregion
        public object Request { get; set; }
        public SqlParameterCollection Parameters { get; set; }
    }
}
