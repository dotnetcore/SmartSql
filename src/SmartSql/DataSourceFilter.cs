using Microsoft.Extensions.Logging;
using SmartSql.Abstractions;
using SmartSql.Abstractions.DataSource;
using SmartSql.Abstractions.DbSession;
using SmartSql.Exceptions;
using SmartSql.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartSql
{
    public class DataSourceFilter : IDataSourceFilter
    {
        private readonly ILogger _logger;
        private readonly IDbConnectionSessionStore _dbSessionStore;
        private readonly SmartSqlContext _smartSqlContext;
        /// <summary>
        /// 权重筛选器
        /// </summary>
        private WeightFilter<IReadDataSource> _weightFilter = new WeightFilter<IReadDataSource>();
        public DataSourceFilter(ILogger<DataSourceFilter> logger
            , IDbConnectionSessionStore dbSessionStore
            , SmartSqlContext smartSqlContext)
        {
            _logger = logger;
            _dbSessionStore = dbSessionStore;
            _smartSqlContext = smartSqlContext;
        }

        public IDataSource Elect(RequestContext context)
        {
            if (_dbSessionStore.LocalSession != null)
            {
                return _dbSessionStore.LocalSession.DataSource;
            }
            return GetDataSource(context);
        }

        private IDataSource GetDataSource(RequestContext context)
        {
            var sourceChoice = context.DataSourceChoice;
            IDataSource choiceDataSource = _smartSqlContext.Database.WriteDataSource;
            var readDataSources = _smartSqlContext.Database.ReadDataSources;
            if (sourceChoice == DataSourceChoice.Read
                && readDataSources != null
                && readDataSources.Count > 0
                )
            {
                if (!String.IsNullOrEmpty(context.ReadDb))
                {
                    choiceDataSource = readDataSources.FirstOrDefault(readDb => readDb.Name == context.ReadDb);
                    if (choiceDataSource == null)
                    {
                        throw new SmartSqlException($"Statement.Key:{context.StatementKey},can not find ReadDb:{context.ReadDb} .");
                    }
                }
                else
                {
                    var seekList = readDataSources.Select(readDataSource => new WeightFilter<IReadDataSource>.WeightSource
                    {
                        Source = readDataSource,
                        Weight = readDataSource.Weight
                    });
                    choiceDataSource = _weightFilter.Elect(seekList).Source;
                }
            }
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"DataSourceFilter GetDataSource Choice: {choiceDataSource.Name} .");
            }
            return choiceDataSource;
        }
    }
}
