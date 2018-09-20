using System;
using System.Collections.Generic;
using SmartSql.Abstractions;
using SmartSql.Exceptions;

namespace SmartSql
{
    public class MapperContainer : IDisposable
    {
        private MapperContainer() { }
        public static MapperContainer Instance = new MapperContainer();
        /// <summary>
        /// Mapper容器
        /// </summary>
        public IDictionary<String, ISmartSqlMapper> Container { get; private set; } = new Dictionary<String, ISmartSqlMapper>();

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
            if (!Container.ContainsKey(smartSqlOptions.Alias))
            {
                lock (this)
                {
                    if (!Container.ContainsKey(smartSqlOptions.Alias))
                    {
                        ISmartSqlMapper _mapper = new SmartSqlMapper(smartSqlOptions);
                        Container.Add(smartSqlOptions.Alias, _mapper);
                    }
                }
            }
            return Container[smartSqlOptions.Alias];
        }

        public ISmartSqlMapper GetSqlMapperByAlias(string alias)
        {
            if (!Container.ContainsKey(alias))
            {
                throw new SmartSqlException($"Can not find ISmartSqlMapper.Alias:{alias}!");
            }
            
            return Container[alias];
        }
        public void Dispose()
        {
            foreach (var mapper in Container)
            {
                mapper.Value.Dispose();
            }
            Container.Clear();
        }
    }
}
