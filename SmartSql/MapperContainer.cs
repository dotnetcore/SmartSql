using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SmartSql.Abstractions;
using SmartSql.Abstractions.Config;
using Microsoft.Extensions.Logging;
using SmartSql.Abstractions.Logging;

namespace SmartSql
{
    public class MapperContainer : IDisposable
    {
        private MapperContainer() { }
        public static MapperContainer Instance = new MapperContainer();

        /// <summary>
        /// Mapper容器
        /// </summary>
        private IDictionary<String, ISmartSqlMapper> _mapperContainer = new Dictionary<String, ISmartSqlMapper>();

        public ISmartSqlMapper GetSqlMapper(String smartSqlMapConfigPath = "SmartSqlMapConfig.xml")
        {
            return GetSqlMapper(NullLoggerFactory.Instance, smartSqlMapConfigPath);
        }
        public ISmartSqlMapper GetSqlMapper(ILoggerFactory loggerFactory, String smartSqlMapConfigPath = "SmartSqlMapConfig.xml")
        {
            return GetSqlMapper(loggerFactory, smartSqlMapConfigPath, new LocalFileConfigLoader(smartSqlMapConfigPath, loggerFactory));
        }
        public ISmartSqlMapper GetSqlMapper(ILoggerFactory loggerFactory, String smartSqlMapConfigPath, IConfigLoader configLoader)
        {
            if (!_mapperContainer.ContainsKey(smartSqlMapConfigPath))
            {
                lock (this)
                {
                    if (!_mapperContainer.ContainsKey(smartSqlMapConfigPath))
                    {
                        ISmartSqlMapper _mapper = new SmartSqlMapper(loggerFactory, smartSqlMapConfigPath, configLoader);
                        _mapperContainer.Add(smartSqlMapConfigPath, _mapper);
                    }
                }
            }
            return _mapperContainer[smartSqlMapConfigPath];
        }

        public void Dispose()
        {
            foreach (var mapper in _mapperContainer)
            {
                mapper.Value.Dispose();
            }
            _mapperContainer.Clear();
        }
    }
}
