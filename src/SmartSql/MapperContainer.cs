using System;
using System.Collections.Generic;
using SmartSql.Abstractions;

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

        public ISmartSqlMapper GetSqlMapper(String smartSqlMapConfigPath = Consts.DEFAULT_SMARTSQL_CONFIG_PATH)
        {
            return GetSqlMapper(new SmartSqlOptions { Alias = smartSqlMapConfigPath, ConfigPath = smartSqlMapConfigPath });
        }

        public ISmartSqlMapper GetSqlMapper(SmartSqlOptions smartSqlOptions)
        {
            if (String.IsNullOrEmpty(smartSqlOptions.Alias))
            {
                smartSqlOptions.Alias = smartSqlOptions.ConfigPath;
            }
            if (!_mapperContainer.ContainsKey(smartSqlOptions.Alias))
            {
                lock (this)
                {
                    if (!_mapperContainer.ContainsKey(smartSqlOptions.Alias))
                    {
                        ISmartSqlMapper _mapper = new SmartSqlMapper(smartSqlOptions);
                        _mapperContainer.Add(smartSqlOptions.Alias, _mapper);
                    }
                }
            }
            return _mapperContainer[smartSqlOptions.Alias];
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
