using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using SmartSql.DataSource;
using SmartSql.Exceptions;
using SmartSql.Utils;

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
        private WeightFilter<ReadDataSource> _weightFilter = new WeightFilter<ReadDataSource>();
        public IgnoreDataSourceChoiceFilter(ILogger<IgnoreDataSourceChoiceFilter> logger)
        {
            _logger = logger;
        }
        public AbstractDataSource Elect(AbstractRequestContext context)
        {
            AbstractDataSource dataSource = !String.IsNullOrEmpty(context.ReadDb) ? FindByDbName(context) : FindDefault(context);
            if (dataSource == null)
            {
                throw new SmartSqlException($"Statement.Id:{context.FullSqlId},can not find ReadDb:{context.ReadDb} .");
            }
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"IgnoreDataSourceChoiceFilter GetDataSource Choice: {dataSource.Name} .");
            }
            return dataSource;
        }

        private AbstractDataSource FindDefault(AbstractRequestContext context)
        {
            var database = context.ExecutionContext.SmartSqlConfig.Database;
            AbstractDataSource dataSource = null;
            if (context.DataSourceChoice == DataSourceChoice.Write)
            {
                dataSource = database.Write;
            }
            else if (database.Reads != null)
            {
                var seekList = database.Reads.Select(readDataSource => new WeightFilter<ReadDataSource>.WeightSource
                {
                    Source = readDataSource.Value,
                    Weight = readDataSource.Value.Weight
                });
                dataSource = _weightFilter.Elect(seekList).Source;
            }
            return dataSource;
        }

        private static AbstractDataSource FindByDbName(AbstractRequestContext context)
        {
            var database = context.ExecutionContext.SmartSqlConfig.Database;
            AbstractDataSource dataSource;
            if (database.Write.Name == context.ReadDb)
            {
                dataSource = database.Write;
            }
            else
            {
                if (!database.Reads.TryGetValue(context.ReadDb, out var readDataSource))
                {
                    throw new SmartSqlException($"Statement.Id:{context.FullSqlId},can not find DbName:{context.ReadDb} .");
                }
                dataSource = readDataSource;
            }

            return dataSource;
        }
    }
}
