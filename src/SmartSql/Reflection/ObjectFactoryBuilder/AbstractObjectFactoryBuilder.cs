using SmartSql.Exceptions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using SmartSql.Utils;

namespace SmartSql.Reflection.ObjectFactoryBuilder
{
    public abstract class AbstractObjectFactoryBuilder : IObjectFactoryBuilder
    {
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
            return CacheUtil<AbstractObjectFactoryBuilder, String, Func<object[], object>>
                  .GetOrAdd(cacheKey, _ => Build(targetType, ctorArgTypes));
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
