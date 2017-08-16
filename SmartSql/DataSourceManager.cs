using SmartSql.Abstractions.DataSource;
using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Abstractions;
using SmartSql.Common;
using System.Linq;
using SmartSql.Abstractions.DbSession;
using Microsoft.Extensions.Logging;

namespace SmartSql
{
    /// <summary>
    /// 数据源管理器
    /// </summary>
    public class DataSourceManager : IDataSourceManager
    {
        private readonly ILogger _logger;
        public ISmartSqlMapper SmartSqlMapper { get; }

        /// <summary>
        /// 权重筛选器
        /// </summary>
        private WeightFilter<IReadDataSource> weightFilter = new WeightFilter<IReadDataSource>();
        public DataSourceManager(ILoggerFactory loggerFactory, ISmartSqlMapper sqlMaper)
        {
            _logger = loggerFactory.CreateLogger<DataSourceManager>();
            SmartSqlMapper = sqlMaper;
        }
        public IDataSource GetDataSource(DataSourceChoice sourceChoice)
        {
            IDataSource choiceDataSource = SmartSqlMapper.SqlMapConfig.Database.WriteDataSource;
            var readDataSources = SmartSqlMapper.SqlMapConfig.Database.ReadDataSources;
            if (sourceChoice == DataSourceChoice.Read
                && readDataSources.Count > 0
                )
            {
                var seekList = readDataSources.Select(readDataSource => new WeightFilter<IReadDataSource>.WeightSource
                {
                    Source = readDataSource,
                    Weight = readDataSource.Weight
                });
                choiceDataSource = weightFilter.Elect(seekList).Source;
            }
            _logger.LogDebug($"DataSourceManager GetDataSource Choice: {choiceDataSource.Name} .");
            return choiceDataSource;
        }
    }
}
