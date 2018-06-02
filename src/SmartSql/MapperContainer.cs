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

        public ISmartSqlMapper GetSqlMapper(String smartSqlMapConfigPath = "SmartSqlMapConfig.xml")
        {
            return GetSqlMapper(new SmartSqlOptions { ConfigPath = smartSqlMapConfigPath });
        }

        public ISmartSqlMapper GetSqlMapper(SmartSqlOptions smartSqlOptions)
        {
            if (!_mapperContainer.ContainsKey(smartSqlOptions.ConfigPath))
            {
                lock (this)
                {
                    if (!_mapperContainer.ContainsKey(smartSqlOptions.ConfigPath))
                    {
                        ISmartSqlMapper _mapper = new SmartSqlMapper(smartSqlOptions);
                        _mapperContainer.Add(smartSqlOptions.ConfigPath, _mapper);
                    }
                }
            }
            return _mapperContainer[smartSqlOptions.ConfigPath];
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
