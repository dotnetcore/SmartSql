using SmartSql.Configuration;
using SmartSql.Exceptions;
using System;
using System.Collections.Generic;

namespace SmartSql
{
    public class SmartSqlContainer : IDisposable
    {
        private SmartSqlContainer() { }
        public static SmartSqlContainer Instance = new SmartSqlContainer();
        /// <summary>
        /// Mapper容器
        /// </summary>
        private Dictionary<String, SmartSqlBuilder> Container { get; set; } = new Dictionary<String, SmartSqlBuilder>();

        public SmartSqlBuilder GetSmartSql(string alias)
        {
            if (alias == null)
            {
                throw new ArgumentNullException(nameof(alias));
            }

            Container.TryGetValue(alias, out var smartSqlBuilder);
            return smartSqlBuilder;
        }
        public void Register(SmartSqlBuilder smartSqlBuilder)
        {
            lock (this)
            {
                if (Container.ContainsKey(smartSqlBuilder.Alias))
                {
                    throw new SmartSqlException($"SmartSql.Alias:[{smartSqlBuilder.Alias}] already exist.");
                }
                Container.Add(smartSqlBuilder.Alias, smartSqlBuilder);
            }
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
