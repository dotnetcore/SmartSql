using System;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using SmartSql.Configuration;
using SmartSql.Utils;
using System.Data.Common;
using SmartSql.TypeHandlers;
using Microsoft.Extensions.Logging;
using System.Data;
using SmartSql.CUD;
using SmartSql.Data;
using SmartSql.Middlewares.Filters;

namespace SmartSql.Middlewares
{
    public class PrepareStatementMiddleware : AbstractMiddleware
    {
        private ILogger _logger;
        private SqlParamAnalyzer _sqlParamAnalyzer;
        private DbProviderFactory _dbProviderFactory;
        private TypeHandlerFactory _typeHandlerFactory;

        protected override Type FilterType => typeof(IPrepareStatementFilter);

        protected override void SelfInvoke<TResult>(ExecutionContext executionContext)
        {
            InitParameters(executionContext);
        }

        #region Init Parameter

        private void InitParameters(ExecutionContext executionContext)
        {
            BuildSql(executionContext.Request);
            BuildDbParameters(executionContext.Request);
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(executionContext.FormatSql(true));
            }
        }

        private void BuildDbParameters(AbstractRequestContext reqConetxt)
        {
            if (reqConetxt.CommandType == CommandType.StoredProcedure)
            {
                foreach (var sqlParameter in reqConetxt.Parameters.Values)
                {
                    if (sqlParameter.SourceParameter != null)
                    {
                        sqlParameter.OnSetSourceParameter.Invoke(sqlParameter);
                        continue;
                    }

                    var sourceParam = _dbProviderFactory.CreateParameter();
                    InitSourceDbParameter(sourceParam, sqlParameter);
                    sourceParam.ParameterName = sqlParameter.Name;
                    sourceParam.Value = sqlParameter.Value;
                    sqlParameter.TypeHandler?.SetParameter(sourceParam, sqlParameter.Value);
                    sqlParameter.SourceParameter = sourceParam;
                }
            }
            else
            {
                reqConetxt.RealSql = _sqlParamAnalyzer.Replace(reqConetxt.RealSql, (paramName, nameWithPrefix) =>
                {
                    if (!reqConetxt.Parameters.TryGetValue(paramName, out var sqlParameter))
                    {
                        return nameWithPrefix;
                    }

                    ITypeHandler typeHandler =
                        (reqConetxt.ParameterMap?.GetParameter(paramName)?.Handler ?? sqlParameter.TypeHandler) ??
                        _typeHandlerFactory.GetTypeHandler(sqlParameter.ParameterType);

                    var sourceParam = _dbProviderFactory.CreateParameter();
                    InitSourceDbParameter(sourceParam, sqlParameter);
                    sourceParam.ParameterName = sqlParameter.Name;
                    typeHandler.SetParameter(sourceParam, sqlParameter.Value);
                    sqlParameter.SourceParameter = sourceParam;
                    if (sqlParameter.Name != paramName)
                    {
                        return
                            $"{reqConetxt.ExecutionContext.SmartSqlConfig.Database.DbProvider.ParameterPrefix}{sqlParameter.Name}";
                    }

                    return nameWithPrefix;
                });
            }
        }

        private void InitSourceDbParameter(DbParameter sourceParam, SqlParameter sqlParameter)
        {
            if (sqlParameter.DbType.HasValue)
            {
                sourceParam.DbType = sqlParameter.DbType.Value;
            }

            if (sqlParameter.Direction.HasValue)
            {
                sourceParam.Direction = sqlParameter.Direction.Value;
            }

            if (sqlParameter.Precision.HasValue)
            {
                sourceParam.Precision = sqlParameter.Precision.Value;
            }

            if (sqlParameter.Scale.HasValue)
            {
                sourceParam.Scale = sqlParameter.Scale.Value;
            }

            if (sqlParameter.Size.HasValue)
            {
                sourceParam.Size = sqlParameter.Size.Value;
            }
        }

        private void BuildSql(AbstractRequestContext requestContext)
        {
            if (!requestContext.IsStatementSql)
            {
                return;
            }

            requestContext.SqlBuilder = new StringBuilder();
            requestContext.Statement.BuildSql(requestContext);
            requestContext.RealSql = requestContext.SqlBuilder.ToString().Trim();
        }

        #endregion

        protected override Task SelfInvokeAsync<TResult>(ExecutionContext executionContext)
        {
            InitParameters(executionContext);
            return Task.CompletedTask;
        }

        public override void SetupSmartSql(SmartSqlBuilder smartSqlBuilder)
        {
            InitFilters(smartSqlBuilder);
            _logger = smartSqlBuilder.SmartSqlConfig.LoggerFactory.CreateLogger<PrepareStatementMiddleware>();
            _sqlParamAnalyzer = smartSqlBuilder.SmartSqlConfig.SqlParamAnalyzer;
            _dbProviderFactory = smartSqlBuilder.SmartSqlConfig.Database.DbProvider.Factory;
            _typeHandlerFactory = smartSqlBuilder.SmartSqlConfig.TypeHandlerFactory;
        }

        public override int Order => 100;
    }
}