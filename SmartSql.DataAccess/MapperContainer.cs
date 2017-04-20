using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SmartSql.Abstractions;
namespace SmartSql.DataAccess
{
    public class MapperContainer : IDisposable
    {
        private MapperContainer() { }
        public static MapperContainer Instance = new MapperContainer();

        /// <summary>
        /// Mapper容器
        /// </summary>
        private IDictionary<String, ISmartSqlMapper> _mapperContainer = new Dictionary<String, ISmartSqlMapper>();

        public ISmartSqlMapper GetSqlMapper(String SmartSqlMapConfigPath = "SmartSqlMapConfig.xml")
        {
            if (!_mapperContainer.ContainsKey(SmartSqlMapConfigPath))
            {
                lock (this)
                {
                    if (!_mapperContainer.ContainsKey(SmartSqlMapConfigPath))
                    {
                        ISmartSqlMapper _mapper = new SmartSqlMapper(SmartSqlMapConfigPath);
                        _mapperContainer.Add(SmartSqlMapConfigPath, _mapper);
                    }
                }
            }
            return _mapperContainer[SmartSqlMapConfigPath];
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
