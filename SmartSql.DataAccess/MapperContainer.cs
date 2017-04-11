using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SmartSql.Abstractions;
namespace SmartSql.DataAccess
{
    public class MapperContainer
    {
        /// <summary>
        /// Mapper容器
        /// </summary>
        private static IDictionary<String, ISmartSqlMapper> _mapperContainer = new Dictionary<String, ISmartSqlMapper>();
        /// <summary>
        /// 同步锁
        /// </summary>
        private static readonly object syncRoot = new object();

        public static ISmartSqlMapper GetInstance(String SmartSqlMapConfigPath = "SmartSqlMapConfig.xml")
        {
            if (_mapperContainer[SmartSqlMapConfigPath] == null)
            {
                lock (syncRoot)
                {
                    if (_mapperContainer[SmartSqlMapConfigPath] == null)
                    {
                        ISmartSqlMapper _mapper = new SmartSqlMapper(SmartSqlMapConfigPath);
                        _mapperContainer.Add(SmartSqlMapConfigPath, _mapper);
                    }
                }
            }
            return _mapperContainer[SmartSqlMapConfigPath];
        }
    }
}
