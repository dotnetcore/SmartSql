using SmartSql.Configuration;
using SmartSql.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
namespace SmartSql
{
    public class SmartSqlContainer : IDisposable
    {
        private SmartSqlContainer() { }
        public static SmartSqlContainer Instance = new SmartSqlContainer();
        /// <summary>
        /// Mapper容器
        /// </summary>
        private ConcurrentDictionary<String, SmartSqlBuilder> Container { get; set; } = new ConcurrentDictionary<String, SmartSqlBuilder>();

        public SmartSqlBuilder GetSmartSql(string alias)
        {
            if (alias == null)
            {
                throw new ArgumentNullException(nameof(alias));
            }

            Container.TryGetValue(alias, out var smartSqlBuilder);
            return smartSqlBuilder;
        }
        public bool TryRegister( SmartSqlBuilder smartSqlBuilder)
        {
            return Container.TryAdd(smartSqlBuilder.Alias, smartSqlBuilder);
        }
        public void Dispose()
        {
            foreach (var smartSqlBuilder in Container.Values)
            {
                smartSqlBuilder.Dispose();
            }
            Container.Clear();
        }
    }
}
