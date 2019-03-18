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

namespace SmartSql.Middlewares
{
    public class PrepareStatementMiddleware : IMiddleware
    {
        private readonly ILogger _logger;
        private readonly SqlParamAnalyzer _sqlParamAnalyzer;
        private readonly DbProviderFactory _dbProviderFactory;
        private readonly ITypeHandlerFactory _typeHandlerFactory;
        public IMiddleware Next { get; set; }
        public PrepareStatementMiddleware(SmartSqlConfig smartSqlConfig)
        {
            _logger = smartSqlConfig.LoggerFactory.CreateLogger<PrepareStatementMiddleware>();
            _sqlParamAnalyzer = smartSqlConfig.SqlParamAnalyzer;
            _dbProviderFactory = smartSqlConfig.Database.DbProvider.Factory;
            _typeHandlerFactory = smartSqlConfig.TypeHandlerFactory;
        }
        public void Invoke<TResult>(ExecutionContext executionContext)
        {
            InitParameters(executionContext);
            Next.Invoke<TResult>(executionContext);
        }
        #region Init Parameter
        private void InitParameters(ExecutionContext executionContext)
        {
            BuildSql(executionContext.Request);
            BuildDbParameters(executionContext.Request);
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                var sourceParameters = executionContext.Request.Parameters.DbParameters.Values;
                var cmdText = executionContext.Request.RealSql;
                string dbParameterStr = string.Join(",", sourceParameters.Select(p => $"{p.ParameterName}={p.Value}"));
                string realSql = _sqlParamAnalyzer.Replace(cmdText, (paramName, nameWithPrefix) =>
                {
                    var paramNameCompare = executionContext.SmartSqlConfig.Settings.IgnoreParameterCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
                    var dbParam = sourceParameters.FirstOrDefault(m => String.Equals(m.ParameterName, paramName, paramNameCompare));
                    if (dbParam == null) { return nameWithPrefix; }
                    if (dbParam.Value == DBNull.Value) { return "NULL"; }
                    switch (dbParam.DbType)
                    {
                        case DbType.AnsiString:
                        case DbType.AnsiStringFixedLength:
                        case DbType.DateTime:
                        case DbType.DateTime2:
                        case DbType.DateTimeOffset:
                        case DbType.Guid:
                        case DbType.String:
                        case DbType.StringFixedLength:
                        case DbType.Time:
                        case DbType.Xml:
                            { return $"'{dbParam.Value}'"; }
                        case DbType.Boolean:
                            {
                                return ((bool)dbParam.Value) ? "1" : "0";
                            }
                    }
                    return dbParam.Value.ToString();
                });
                _logger.LogDebug($"Statement.Id:[{executionContext.Request.FullSqlId}],Sql:{Environment.NewLine}{cmdText}{Environment.NewLine}Parameters:[{dbParameterStr}]{Environment.NewLine}Sql with parameter value: {Environment.NewLine}{realSql}");
            }
        }

        private void BuildDbParameters(RequestContext reqConetxt)
        {
            var dbParameterNames = _sqlParamAnalyzer.Analyse(reqConetxt.RealSql);
            foreach (var paramName in dbParameterNames)
            {
                if (!reqConetxt.Parameters.TryGetValue(paramName, out var sqlParameter))
                {
                    continue;
                }
                var sourceParam = _dbProviderFactory.CreateParameter();
                sourceParam.ParameterName = paramName;
                if (sqlParameter.TypeHandler == null)
                {
                    sqlParameter.TypeHandler = _typeHandlerFactory.Get(sqlParameter.ParameterType);
                }
                var typeHandler = sqlParameter.TypeHandler;
                typeHandler.SetParameter(sourceParam, sqlParameter.Value);
                sqlParameter.SourceParameter = sourceParam;
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
        }

        private void BuildSql(RequestContext requestContext)
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
        public async Task InvokeAsync<TResult>(ExecutionContext executionContext)
        {
            InitParameters(executionContext);
            await Next.InvokeAsync<TResult>(executionContext);
        }
    }
}
