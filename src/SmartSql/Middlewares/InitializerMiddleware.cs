using SmartSql.Configuration;
using SmartSql.Data;
using SmartSql.DataSource;
using System;
using System.Threading.Tasks;
using SmartSql.AutoConverter;
using SmartSql.Exceptions;

namespace SmartSql.Middlewares
{
    public class InitializerMiddleware : AbstractMiddleware
    {
        private SmartSqlConfig _smartSqlConfig;

        public override void Invoke<TResult>(ExecutionContext executionContext)
        {
            Init(executionContext);
            InvokeNext<TResult>(executionContext);
        }

        public override async Task InvokeAsync<TResult>(ExecutionContext executionContext)
        {
            Init(executionContext);
            await InvokeNextAsync<TResult>(executionContext);
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

            SqlMap sqlMap = null;

            if (!String.IsNullOrEmpty(requestContext.Scope))
            {
                sqlMap = _smartSqlConfig.GetSqlMap(requestContext.Scope);
            }

            if (requestContext.IsStatementSql)
            {
                InitByStatement(requestContext, sqlMap);
            }
            else
            {
                InitByMap(requestContext, sqlMap);
            }

            if (requestContext.AutoConverter == null)
            {
                requestContext.AutoConverter = NoneAutoConverter.INSTANCE;
            }
        }

        private void InitByStatement(AbstractRequestContext requestContext, SqlMap sqlMap)
        {
            requestContext.Statement = sqlMap.GetStatement(requestContext.FullSqlId);
            if (requestContext.DataSourceChoice == DataSourceChoice.Unknow)
            {
                if (requestContext.Statement.SourceChoice.HasValue)
                {
                    requestContext.DataSourceChoice = requestContext.Statement.SourceChoice.Value;
                }
                else
                {
                    requestContext.DataSourceChoice = (StatementType.Write & requestContext.Statement.StatementType)
                                                      != StatementType.Unknown
                        ? DataSourceChoice.Write
                        : DataSourceChoice.Read;
                }
            }

            if (requestContext.Statement.CommandType.HasValue)
            {
                requestContext.CommandType = requestContext.Statement.CommandType.Value;
            }

            if (!requestContext.EnablePropertyChangedTrack.HasValue)
            {
                requestContext.EnablePropertyChangedTrack = requestContext.Statement.EnablePropertyChangedTrack;
            }

            requestContext.Transaction = requestContext.Transaction ?? requestContext.Statement.Transaction;
            requestContext.CommandTimeout = requestContext.CommandTimeout ?? requestContext.Statement.CommandTimeout;
            if (String.IsNullOrEmpty(requestContext.ReadDb))
            {
                requestContext.ReadDb = requestContext.Statement.ReadDb;
            }

            if (String.IsNullOrEmpty(requestContext.CacheId))
            {
                requestContext.CacheId = requestContext.Statement.CacheId;
                requestContext.Cache = requestContext.Statement.Cache;
            }
            else
            {
                SetCache(requestContext, sqlMap);
            }

            if (!String.IsNullOrEmpty(requestContext.AutoConverterName))
            {
                if (!_smartSqlConfig.AutoConverters.TryGetValue(requestContext.AutoConverterName, out var autoConverter)
                )
                {
                    throw new SmartSqlException(
                        $"The auto converter with name {requestContext.AutoConverterName} was not found");
                }

                requestContext.AutoConverter = autoConverter;
            }
            else if (requestContext.Statement.AutoConverter != null)
            {
                requestContext.AutoConverter = requestContext.Statement.AutoConverter;
            }
            else
            {
                requestContext.AutoConverter = sqlMap.AutoConverter;
            }

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
                SetCache(requestContext, sqlMap);
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

            if (!String.IsNullOrEmpty(requestContext.AutoConverterName))
            {
                if (!_smartSqlConfig.AutoConverters.TryGetValue(requestContext.AutoConverterName, out var autoConverter)
                )
                {
                    throw new SmartSqlException(
                        $"The auto converter with name {requestContext.AutoConverterName} was not found");
                }

                requestContext.AutoConverter = autoConverter;
            }
            else if (requestContext.Statement?.AutoConverter != null)
            {
                requestContext.AutoConverter = requestContext.Statement.AutoConverter;
            }
            else
            {
                requestContext.AutoConverter = sqlMap?.AutoConverter;
            }
        }

        private static void SetCache(AbstractRequestContext requestContext, SqlMap sqlMap)
        {
            string fullCacheId;
            if (!requestContext.CacheId.Contains("."))
            {
                fullCacheId = $"{requestContext.Scope}.{requestContext.CacheId}";
                requestContext.Cache = sqlMap.GetCache(fullCacheId);
            }
            else
            {
                fullCacheId = requestContext.CacheId;
                var fullCacheIdSplit = fullCacheId.Split('.');
                if (fullCacheIdSplit.Length != 2)
                {
                    throw new SmartSqlException($"Wrong CacheId:[{requestContext.CacheId}]");
                }

                var scope = fullCacheIdSplit[0];
                requestContext.Cache = sqlMap.Scope == scope
                    ? sqlMap.GetCache(fullCacheId)
                    : sqlMap.SmartSqlConfig.GetSqlMap(scope).GetCache(fullCacheId);
            }
        }

        private void InitParameterCollection(AbstractRequestContext requestContext)
        {
            requestContext.SetupParameters();
        }

        public override void SetupSmartSql(SmartSqlBuilder smartSqlBuilder)
        {
            _smartSqlConfig = smartSqlBuilder.SmartSqlConfig;
        }

        public override int Order => 0;
    }
}