using SmartSql.Exceptions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SmartSql.Reflection.ObjectFactoryBuilder
{
    public abstract class AbstractObjectFactoryBuilder : IObjectFactoryBuilder
    {
        private static readonly object _syncObj = new object();
        private static readonly IDictionary<String, Func<object[], object>> _factoryCache = new Dictionary<string, Func<object[], object>>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="ctorArgTypes"></param>
        /// <returns>{targetType.FullName}:{artType.FullName}</returns>
        protected string GetCacheKey(Type targetType, Type[] ctorArgTypes)
        {
            StringBuilder cacheKey = new StringBuilder();
            cacheKey.Append(targetType.FullName);
            if (ctorArgTypes.Length <= 0) return cacheKey.ToString();
            cacheKey.Append(":");
            foreach (var ctorArgType in ctorArgTypes)
            {
                cacheKey.Append(ctorArgType.FullName).Append('&');
            }
            return cacheKey.ToString().TrimEnd('&');
        }
        public Func<object[], object> GetObjectFactory(Type targetType, Type[] ctorArgTypes)
        {
            if (ctorArgTypes == null)
            {
                ctorArgTypes = Type.EmptyTypes;
            }
            var cacheKey = GetCacheKey(targetType, ctorArgTypes);
            if (!_factoryCache.ContainsKey(cacheKey))
            {
                lock (_syncObj)
                {
                    if (!_factoryCache.ContainsKey(cacheKey))
                    {
                        var newFunc = Build(targetType, ctorArgTypes);
                        _factoryCache.Add(cacheKey, newFunc);
                    }
                }
            }
            return _factoryCache[cacheKey];
        }
        protected ConstructorInfo GetConstructor(Type targetType, Type[] ctorArgTypes)
        {
            var targetCtor = targetType.GetConstructor(ctorArgTypes);
            if (targetCtor == null)
            {
                throw new SmartSqlException($"Can not find Type:{targetType.FullName} ConstructorInfo!");
            }
            return targetCtor;
        }
        public abstract Func<object[], object> Build(Type targetType, Type[] ctorArgTypes);
    }
}
