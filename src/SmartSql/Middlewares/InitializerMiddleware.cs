using SmartSql.Configuration;
using SmartSql.Data;
using SmartSql.DataSource;
using System;
using System.Threading.Tasks;

namespace SmartSql.Middlewares
{
    public class InitializerMiddleware : IMiddleware
    {
        private readonly SmartSqlConfig _smartSqlConfig;
        public IMiddleware Next { get; set; }
        public InitializerMiddleware(SmartSqlConfig smartSqlConfig)
        {
            _smartSqlConfig = smartSqlConfig;
        }
        public void Invoke<TResult>(ExecutionContext executionContext)
        {
            Init(executionContext);
            Next.Invoke<TResult>(executionContext);
        }

        public async Task InvokeAsync<TResult>(ExecutionContext executionContext)
        {
            Init(executionContext);
            await Next.InvokeAsync<TResult>(executionContext);
        }

        private void Init(ExecutionContext executionContext)
        {
            InitRequest(executionContext.Request);
            InitParameterCollection(executionContext.Request);
        }
        private void InitRequest(AbstractRequestContext requestContext)
        {
            if (!String.IsNullOrEmpty(requestContext.RealSql))
            {
                requestContext.IsStatementSql = false;
            }
            if (!String.IsNullOrEmpty(requestContext.Scope))
            {
                var sqlMap = _smartSqlConfig.GetSqlMap(requestContext.Scope);

                if (requestContext.IsStatementSql)
                {
                    InitByStatement(requestContext, sqlMap);
                }
                else
                {
                    InitByMap(requestContext, sqlMap);
                }
            }
        }

        private void InitByStatement(AbstractRequestContext requestContext, SqlMap sqlMap)
        {
            requestContext.Statement = sqlMap.GetStatement(requestContext.FullSqlId);
            if (requestContext.Statement.SourceChoice.HasValue)
            {
                requestContext.DataSourceChoice = requestContext.Statement.SourceChoice.Value;
            }
            else
            {
                requestContext.DataSourceChoice = (StatementType.Write & requestContext.Statement.StatementType)
                                                  != StatementType.Unknown ? DataSourceChoice.Write : DataSourceChoice.Read;
            }
            if (requestContext.Statement.CommandType.HasValue)
            {
                requestContext.CommandType = requestContext.Statement.CommandType.Value;
            }
            requestContext.Transaction = requestContext.Transaction ?? requestContext.Statement.Transaction;
            requestContext.CommandTimeout = requestContext.CommandTimeout ?? requestContext.Statement.CommandTimeout;
            requestContext.ReadDb = requestContext.Statement.ReadDb;
            requestContext.CacheId = requestContext.Statement.CacheId;
            requestContext.Cache = requestContext.Statement.Cache;
            requestContext.ParameterMapId = requestContext.Statement.ParameterMapId;
            requestContext.ParameterMap = requestContext.Statement.ParameterMap;
            requestContext.ResultMapId = requestContext.Statement.ResultMapId;
            requestContext.ResultMap = requestContext.Statement.ResultMap;
            requestContext.MultipleResultMapId = requestContext.Statement.MultipleResultMapId;
            requestContext.MultipleResultMap = requestContext.Statement.MultipleResultMap;
        }
        private void InitByMap(AbstractRequestContext requestContext, SqlMap sqlMap)
        {
            if (!string.IsNullOrEmpty(requestContext.CacheId))
            {
                var fullCacheId = $"{requestContext.Scope}.{requestContext.CacheId}";
                requestContext.Cache = sqlMap.GetCache(fullCacheId);
            }
            if (!String.IsNullOrEmpty(requestContext.ParameterMapId))
            {
                var fullParameterMapIdId = $"{requestContext.Scope}.{requestContext.ParameterMapId}";
                requestContext.ParameterMap = sqlMap.GetParameterMap(fullParameterMapIdId);
            }
            if (!String.IsNullOrEmpty(requestContext.ResultMapId))
            {
                var fullResultMapId = $"{requestContext.Scope}.{requestContext.ResultMapId}";
                requestContext.ResultMap = sqlMap.GetResultMap(fullResultMapId);
            }
            if (!String.IsNullOrEmpty(requestContext.MultipleResultMapId))
            {
                var fullMultipleResultMapId = $"{requestContext.Scope}.{requestContext.MultipleResultMapId}";
                requestContext.MultipleResultMap = sqlMap.GetMultipleResultMap(fullMultipleResultMapId);
            }
        }
        private void InitParameterCollection(AbstractRequestContext requestContext)
        {
            requestContext.SetupParameters();
        }
    }
}
