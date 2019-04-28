using SmartSql.Configuration;
using SmartSql.DataSource;
using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using SmartSql.Data;
using SmartSql.Reflection.Convert;
using SmartSql.TypeHandlers;

namespace SmartSql
{
    public abstract class AbstractRequestContext
    {
        public ExecutionContext ExecutionContext { get; internal set; }
        public ExecutionType ExecutionType { get; set; }
        public DataSourceChoice DataSourceChoice { get; set; } = DataSourceChoice.Unknow;
        public CommandType CommandType { get; set; } = CommandType.Text;
        public IsolationLevel? Transaction { get; set; }
        public String ReadDb { get; set; }
        public int? CommandTimeout { get; set; }
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
        public string ParameterMapId { get; set; }
        public ParameterMap ParameterMap { get; internal set; }
        public string ResultMapId { get; set; }
        public ResultMap ResultMap { get; internal set; }
        public string MultipleResultMapId { get; set; }
        public MultipleResultMap MultipleResultMap { get; internal set; }
        #endregion
        public SqlParameterCollection Parameters { get; set; }

        public ResultMap GetCurrentResultMap()
        {
            return MultipleResultMap != null ?
                MultipleResultMap.Results[ExecutionContext.DataReaderWrapper.ResultIndex]?.Map : ResultMap;
        }

        public abstract void SetupParameters();
        public abstract void SetRequest(object requestObj);
    }

    public class RequestContext<TRequest> : AbstractRequestContext where TRequest : class
    {
        public TRequest Request { get; set; }

        public override void SetupParameters()
        {
            Parameters = SqlParameterCollection.Create<TRequest>(this);
        }

        public override void SetRequest(object requestObj)
        {
            Request = (TRequest) requestObj;
        }
    }

    public class RequestContext : RequestContext<object>
    {

    }
}
