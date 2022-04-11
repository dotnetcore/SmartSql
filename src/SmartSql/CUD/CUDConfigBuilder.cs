using System;
using System.Collections.Generic;
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

        public CUDConfigBuilder(IEnumerable<Type> entityTypeList)
            : this(new NativeConfigBuilder(new SmartSqlConfig()), entityTypeList)
        {
        }

        public CUDConfigBuilder(IConfigBuilder parent, IEnumerable<Type> entityTypeList)
        {
            Parent = parent;
            _entityTypeList = entityTypeList;
        }

        public SmartSqlConfig Build()
        {
            if (Initialized)
            {
                return SmartSqlConfig;
            }

            SmartSqlConfig = Parent.Build();

            foreach (var entityType in _entityTypeList)
            {
                var scope = EntityMetaDataCacheType.GetTableName(entityType);
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
                //TODO Generate CUD Statement based on Type
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