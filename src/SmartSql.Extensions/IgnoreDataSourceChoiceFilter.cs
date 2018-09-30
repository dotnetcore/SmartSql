using Microsoft.Extensions.Logging;
using SmartSql.Abstractions;
using SmartSql.Abstractions.DataSource;
using SmartSql.Exceptions;
using SmartSql.Utils;
using System;
using System.Linq;

namespace SmartSql.Extensions
{
    /// <summary>
    /// Ignore DataSourceChoiceFilter
    /// Warning: It is not recommended to use!!!
    /// </summary>
    public class IgnoreDataSourceChoiceFilter : IDataSourceFilter
    {
        private readonly ILogger<IgnoreDataSourceChoiceFilter> _logger;
        /// <summary>
        /// 权重筛选器
        /// </summary>
        private WeightFilter<IReadDataSource> _weightFilter = new WeightFilter<IReadDataSource>();
        public IgnoreDataSourceChoiceFilter(ILogger<IgnoreDataSourceChoiceFilter> logger)
        {
            _logger = logger;
        }
        public IDataSource Elect(RequestContext context)
        {
            IDataSource dataSource;
            if (!String.IsNullOrEmpty(context.ReadDb))
            {
                dataSource = FindByDbName(context);
            }
            else
            {
                dataSource = FindDefault(context);
            }
            if (dataSource == null)
            {
                throw new SmartSqlException($"Statement.Key:{context.StatementKey},can not find ReadDb:{context.ReadDb} .");
            }
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"IgnoreDataSourceChoiceFilter GetDataSource Choice: {dataSource.Name} .");
            }
            return dataSource;
        }

        private IDataSource FindDefault(RequestContext context)
        {
            var database = context.SmartSqlContext.Database;
            IDataSource dataSource = default;
            if (context.DataSourceChoice == DataSourceChoice.Write)
            {
                dataSource = database.WriteDataSource;
            }
            else if (database.ReadDataSources != null)
            {
                var seekList = database.ReadDataSources.Select(readDataSource => new WeightFilter<IReadDataSource>.WeightSource
                {
                    Source = readDataSource,
                    Weight = readDataSource.Weight
                });
                dataSource = _weightFilter.Elect(seekList).Source;
            }
            return dataSource;
        }

        private static IDataSource FindByDbName(RequestContext context)
        {
            var database = context.SmartSqlContext.Database;
            IDataSource dataSource;
            if (database.WriteDataSource.Name == context.ReadDb)
            {
                dataSource = database.WriteDataSource;
            }
            else
            {
                dataSource = database.ReadDataSources.FirstOrDefault(m => m.Name == context.ReadDb);
            }
            if (dataSource == null)
            {
                throw new SmartSqlException($"Statement.Key:{context.StatementKey},can not find DbName:{context.ReadDb} .");
            }
            return dataSource;
        }
    }
}
