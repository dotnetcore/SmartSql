using Microsoft.Extensions.Logging;
using SmartSql.Reflection.TypeConstants;
using System;
using System.Collections.Concurrent;

namespace SmartSql.DyRepository
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly ConcurrentDictionary<Type, object>
            _cachedRepository = new ConcurrentDictionary<Type, object>();

        private readonly IRepositoryBuilder _repositoryBuilder;
        private readonly ILogger _logger;

        public RepositoryFactory(IRepositoryBuilder repositoryBuilder
            , ILogger logger
        )
        {
            _repositoryBuilder = repositoryBuilder;
            _logger = logger;
        }

        public object CreateInstance(Type interfaceType, ISqlMapper sqlMapper, string scope = "")
        {
            return _cachedRepository.GetOrAdd(interfaceType, (key) =>
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug(
                        $"RepositoryFactory.CreateInstance :InterfaceType.FullName:[{interfaceType.FullName}] Start");
                }

                var implType = _repositoryBuilder.Build(interfaceType, sqlMapper.SmartSqlConfig, scope);

                var obj = sqlMapper.SmartSqlConfig.ObjectFactoryBuilder
                    .GetObjectFactory(implType, new Type[] { ISqlMapperType.Type })(new object[] { sqlMapper });
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug(
                        $"RepositoryFactory.CreateInstance :InterfaceType.FullName:[{interfaceType.FullName}],ImplType.FullName:[{implType.FullName}] End");
                }

                return obj;
            });
        }
    }
}