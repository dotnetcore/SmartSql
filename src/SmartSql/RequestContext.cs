using SmartSql.Configuration;
using SmartSql.DataSource;
using System;
using System.Data;
using System.Text;
using SmartSql.AutoConverter;
using SmartSql.Cache;
using SmartSql.Data;
using SmartSql.Reflection.EntityProxy;

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
        public bool? EnablePropertyChangedTrack { get; set; }
        public Statement Statement { get; internal set; }
        public StringBuilder SqlBuilder { get; internal set; }
        public bool IsStatementSql { get; internal set; } = true;
        internal bool IgnorePrepend { get; set; } = false;
        public String RealSql { get; set; }

        public String AutoConverterName { get; set; }

        internal IAutoConverter AutoConverter { get; set; }

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

        public ISqlParameterCollection Parameters { get; protected set; }

        public ResultMap GetCurrentResultMap()
        {
            return MultipleResultMap != null
                ? MultipleResultMap.GetResultMap(ExecutionContext.DataReaderWrapper.ResultIndex)
                : ResultMap;
        }

        public String CacheKeyTemplate { get; set; }
        public CacheKey CacheKey { get; private set; }

        public CacheKey EnsureCacheKey()
        {
            if (CacheKey != null)
            {
                return CacheKey;
            }

            if (!String.IsNullOrEmpty(CacheKeyTemplate))
            {
                var key = ExecutionContext.SmartSqlConfig.CacheTemplateAnalyzer.Replace(CacheKeyTemplate,
                    (paramName, nameWithPrefix) =>
                    {
                        if (Parameters.TryGetValue(paramName, out var sqlParameter))
                        {
                            return sqlParameter.Value.ToString();
                        }

                        return nameWithPrefix;
                    });

                CacheKey = new CacheKey(key, ExecutionContext.Result.ResultType);
            }
            else
            {
                CacheKey = new CacheKey(this);
            }

            return CacheKey;
        }

        public abstract void SetupParameters();
        public abstract void SetRequest(object requestObj);
        public abstract Object GetRequest();

        /// <summary>
        /// 获取请求实体变更的版本号
        /// </summary>
        /// <param name="propName">属性名</param>
        /// <returns>变更次数,如果返回值是 -1 则实体对象不为增强后的代理对象</returns>
        public abstract int GetPropertyVersion(string propName);
    }

    public class RequestContext<TRequest> : AbstractRequestContext where TRequest : class
    {
        public TRequest Request { get; set; }

        public override void SetupParameters()
        {
            bool ignoreParameterCase = false;
            if (ExecutionContext != null)
            {
                ignoreParameterCase = ExecutionContext.SmartSqlConfig.Settings.IgnoreParameterCase;
            }

            Parameters =
                SqlParameterCollection.Create<TRequest>(Request, ignoreParameterCase);
        }

        public override void SetRequest(object requestObj)
        {
            Request = (TRequest) requestObj;
        }

        public override object GetRequest()
        {
            return Request;
        }

        public override int GetPropertyVersion(string propName)
        {
            if (Request is IEntityPropertyChangedTrackProxy trackProxy)
            {
                return trackProxy.GetPropertyVersion(propName);
            }

            return -1;
        }
    }

    public class RequestContext : RequestContext<object>
    {
    }
}