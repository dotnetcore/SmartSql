using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SmartSql.ConfigBuilder;
using SmartSql.Configuration;
using SmartSql.Reflection.TypeConstants;

namespace SmartSql.CUD
{
    public class CUDConfigBuilder : IConfigBuilder
    {
        public bool Initialized { get; private set; }
        public SmartSqlConfig SmartSqlConfig { get; private set; }
        public IConfigBuilder Parent { get; }
        private readonly IEnumerable<Type> _entityTypeList;
        private ILoggerFactory _loggerFactory;
        private ILogger _logger;

        public CUDConfigBuilder(IConfigBuilder parent, IEnumerable<Type> entityTypeList, ILoggerFactory loggerFactory = null)
        {
            Parent = parent;
            _entityTypeList = entityTypeList;
            _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
            _logger = _loggerFactory.CreateLogger<XmlConfigBuilder>();
        }

        public SmartSqlConfig Build()
        {
            if (Initialized)
            {
                return SmartSqlConfig;
            }

            SmartSqlConfig = Parent.Build();
            
            var sqlGen = new CUDSqlGenerator(SmartSqlConfig);

            foreach (var entityType in _entityTypeList)
            {
                var scope = EntityMetaDataCacheType.GetScope(entityType);
                if (!SmartSqlConfig.SqlMaps.TryGetValue(scope, out SqlMap sqlMap))
                {
                    sqlMap = new SqlMap
                    {
                        Path = entityType.AssemblyQualifiedName,
                        Scope = scope,
                        SmartSqlConfig = SmartSqlConfig,
                        Statements = new Dictionary<string, Statement>(),
                        Caches = new Dictionary<string, Configuration.Cache>(),
                        MultipleResultMaps = new Dictionary<string, MultipleResultMap>(),
                        ParameterMaps = new Dictionary<string, ParameterMap>(),
                        ResultMaps = new Dictionary<string, ResultMap>()
                    };
                    SmartSqlConfig.SqlMaps.Add(scope, sqlMap);
                }
                var result = sqlGen.Generate(sqlMap, entityType);
                foreach (var statement in result)
                {
                    if (sqlMap.Statements.ContainsKey(statement.Key))
                    {
                        _logger.LogDebug("{0} Exists, CUD module Skip this Statement. Continue", statement.Key);
                        continue;
                    }
                    sqlMap.Statements.Add(statement.Key, statement.Value);
                }
            }

            Initialized = true;
            return SmartSqlConfig;
        }

        public void SetParent(IConfigBuilder configBuilder)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            Parent.Dispose();
        }
    }
}