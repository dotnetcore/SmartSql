using SmartSql.Abstractions.TypeHandler;
using SmartSql.Configuration.Statements;
using SmartSql.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using SmartSql.Configuration.Maps;

namespace SmartSql.Abstractions
{
    /// <summary>
    /// Sql 请求上下文
    /// </summary>
    public class RequestContext
    {
        public SmartSqlContext SmartSqlContext { get; internal set; }
        public DataSourceChoice DataSourceChoice { get; set; } = DataSourceChoice.Unknow;
        public CommandType CommandType { get; set; } = CommandType.Text;
        public String ReadDb { get; set; }
        public Statement Statement { get; internal set; }
        public StringBuilder Sql { get; internal set; }
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
        public DbParameterCollection RequestParameters { get; internal set; }
        public IDictionary<object, object> Items { get; set; }
        #region Map
        public String CacheId { get; set; }
        public Configuration.Cache Cache { get; private set; }
        public string ResultMapId { get; set; }
        public ResultMap ResultMap { get; private set; }
        public string ParameterMapId { get; set; }
        public ParameterMap ParameterMap { get; private set; }
        public string MultipleResultMapId { get; set; }
        public MultipleResultMap MultipleResultMap { get; private set; }
        #endregion
        public object Request { get; set; }
        public RequestIdentity RequestIdentity { get; private set; }
        [Obsolete("Internal call")]
        public void Setup(SmartSqlOptions smartSqlOptions)
        {
            SmartSqlContext = smartSqlOptions.SmartSqlContext;
            SetupStatement();
            SetupParameters();
            SetupMap();
            if (Items == null)
            {
                Items = new Dictionary<object, object>();
            }
            smartSqlOptions.SqlBuilder.BuildSql(this);
            SetupKey();
        }
        internal void SetupStatement()
        {
            if (!String.IsNullOrEmpty(RealSql))
            {
                IsStatementSql = false;
            }
            if (IsStatementSql)
            {
                Statement = SmartSqlContext.GetStatement(FullSqlId);
                if (Statement.SourceChoice.HasValue)
                {
                    DataSourceChoice = Statement.SourceChoice.Value;
                }
                else
                {
                    if (Statement.SqlCommandType.HasFlag(SqlCommandType.Insert)
                        || Statement.SqlCommandType.HasFlag(SqlCommandType.Delete)
                        || Statement.SqlCommandType.HasFlag(SqlCommandType.Update)
                        )
                    {
                        DataSourceChoice = DataSourceChoice.Write;
                    }
                }
                if (Statement.CommandType.HasValue)
                {
                    CommandType = Statement.CommandType.Value;
                }
                if (String.IsNullOrEmpty(ReadDb))
                {
                    ReadDb = Statement.ReadDb;
                }
            }
        }
        internal void SetupParameters()
        {
            bool ignoreParameterCase = SmartSqlContext.IgnoreParameterCase;
            if (Request is DbParameterCollection dbParameterCollection
                && dbParameterCollection.IgnoreParameterCase == ignoreParameterCase
                )
            {
                RequestParameters = dbParameterCollection;
            }
            else
            {
                RequestParameters = new DbParameterCollection(ignoreParameterCase, Request);
            }
        }
        internal void SetupMap()
        {
            if (Statement != null)
            {
                SetupStatementMap();
            }
            else if (!String.IsNullOrEmpty(Scope))
            {
                if (String.IsNullOrEmpty(CacheId))
                {
                    var fullCacheId = $"{Scope}.{CacheId}";
                    Cache = SmartSqlContext.GetCache(fullCacheId);
                }
                if (String.IsNullOrEmpty(ResultMapId))
                {
                    var fullResultMapId = $"{Scope}.{ResultMapId}";
                    ResultMap = SmartSqlContext.GetResultMap(fullResultMapId);
                }
                if (String.IsNullOrEmpty(ParameterMapId))
                {
                    var fullParameterMapId = $"{Scope}.{ParameterMapId}";
                    ParameterMap = SmartSqlContext.GetParameterMap(fullParameterMapId);
                }
                if (String.IsNullOrEmpty(MultipleResultMapId))
                {
                    var fullMultipleResultMapId = $"{Scope}.{MultipleResultMapId}";
                    MultipleResultMap = SmartSqlContext.GetMultipleResultMap(fullMultipleResultMapId);
                }
            }
        }

        private void SetupKey()
        {
            RequestIdentity = new RequestIdentity(this);
        }
        private void SetupStatementMap()
        {
            CacheId = Statement.CacheId;
            Cache = Statement.Cache;
            ResultMapId = Statement.ResultMapId;
            ResultMap = Statement.ResultMap;
            ParameterMapId = Statement.ParameterMapId;
            ParameterMap = Statement.ParameterMap;
            MultipleResultMapId = Statement.MultipleResultMapId;
            MultipleResultMap = Statement.MultipleResultMap;
        }
        public String StatementKey => RequestIdentity.StatementKey;
        [Obsolete("Internal call")]
        public ITypeHandler GetTypeHandler(string typeHandlerName)
        {
            return SmartSqlContext.SqlMapConfig.TypeHandlers.FirstOrDefault(th => th.Name == typeHandlerName)?.Handler;
        }
        internal Parameter GetParameterMap(string paramName)
        {
            return ParameterMap?.Parameters?.FirstOrDefault(p => p.Name == paramName);
        }
        /// <summary>
        /// 根据参数名获取对应实体属性名
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        internal string GetPropertyName(string paramName)
        {
            var paramMap = GetParameterMap(paramName);
            return paramMap != null ? paramMap.Property : paramName;
        }

        internal DbParameter GetDbParameter(string paramName)
        {
            var propertyName = GetPropertyName(paramName);
            return RequestParameters.Get(propertyName);
        }
    }
}
