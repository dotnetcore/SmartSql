using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using SmartSql.Abstractions;
using System.Collections;
using Microsoft.Extensions.Logging;

namespace SmartSql.DyRepository
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private IDictionary<Type, object> _cachedRepository = new Dictionary<Type, object>();

        private readonly IRepositoryBuilder _repositoryBuilder;
        private readonly ILogger<RepositoryFactory> _logger;

        public RepositoryFactory(IRepositoryBuilder repositoryBuilder
            , ILogger<RepositoryFactory> logger
            )
        {
            _repositoryBuilder = repositoryBuilder;
            _logger = logger;
        }

        public object CreateInstance(Type interfaceType, ISmartSqlMapper smartSqlMapper, string scope = "")
        {
            if (!_cachedRepository.ContainsKey(interfaceType))
            {
                lock (this)
                {
                    if (!_cachedRepository.ContainsKey(interfaceType))
                    {
                        if (_logger.IsEnabled(LogLevel.Debug))
                        {
                            _logger.LogDebug($"RepositoryFactory.CreateInstance :InterfaceType.FullName:[{interfaceType.FullName}] Start");
                        }
                        var implType = _repositoryBuilder.BuildRepositoryImpl(interfaceType, smartSqlMapper, scope);
                        var obj = Activator.CreateInstance(implType, new object[] { smartSqlMapper });
                        _cachedRepository.Add(interfaceType, obj);
                        if (_logger.IsEnabled(LogLevel.Debug))
                        {
                            _logger.LogDebug($"RepositoryFactory.CreateInstance :InterfaceType.FullName:[{interfaceType.FullName}],ImplType.FullName:[{implType.FullName}] End");
                        }
                    }
                }
            }
            return _cachedRepository[interfaceType];
        }


        public T CreateInstance<T>(ISmartSqlMapper smartSqlMapper, string scope = "")
        {
            var interfaceType = typeof(T);
            return (T)CreateInstance(interfaceType, smartSqlMapper, scope);
        }
    }
}
