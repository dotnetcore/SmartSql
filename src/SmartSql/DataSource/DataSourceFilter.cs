using SmartSql.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SmartSql.Utils;
using Microsoft.Extensions.Logging;

namespace SmartSql.DataSource
{
    public class DataSourceFilter : IDataSourceFilter
    {
        private readonly ILogger _logger;
        public DataSourceFilter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<DataSourceFilter>();
        }

        /// <summary>
        /// 权重筛选器
        /// </summary>
        private readonly WeightFilter<AbstractDataSource> _weightFilter = new WeightFilter<AbstractDataSource>();

        public AbstractDataSource Elect(AbstractRequestContext context)
        {
            if (context.ExecutionContext.SmartSqlConfig.SessionStore.LocalSession?.DataSource != null)
            {
                return context.ExecutionContext.SmartSqlConfig.SessionStore.LocalSession.DataSource;
            }
            return GetDataSource(context);
        }
        private AbstractDataSource GetDataSource(AbstractRequestContext context)
        {
            var sourceChoice = context.DataSourceChoice;
            var database = context.ExecutionContext.SmartSqlConfig.Database;
            AbstractDataSource choiceDataSource = database.Write;
            var readDataSources = database.Reads;
            if (sourceChoice != DataSourceChoice.Read || readDataSources == null || readDataSources.Count <= 0)
                return choiceDataSource;
            if (!string.IsNullOrEmpty(context.ReadDb))
            {
                if (!readDataSources.TryGetValue(context.ReadDb, out var readDataSource))
                {
                    throw new SmartSqlException($"Can not find ReadDb:{context.ReadDb} .");
                }
                choiceDataSource = readDataSource;
            }
            else
            {
                var seekList = readDataSources.Values.Select(readDataSource => new WeightFilter<AbstractDataSource>.WeightSource
                {
                    Source = readDataSource,
                    Weight = readDataSource.Weight
                });
                choiceDataSource = _weightFilter.Elect(seekList).Source;
            }
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"DataSourceFilter GetDataSource Choice: {choiceDataSource.Name} .");
            }
            return choiceDataSource;
        }
    }
}
