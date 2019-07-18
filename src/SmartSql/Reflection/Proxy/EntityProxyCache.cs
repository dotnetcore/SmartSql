using System;

namespace SmartSql.Reflection.Proxy
{
    public static class EntityProxyCache<TEntity>
        where TEntity : class, new()
    {
        public static Type ProxyType { get; }

        static EntityProxyCache()
        {
            ProxyType = EntityProxyFactory.Instance.CreateProxyType(typeof(TEntity));
        }

        public static TEntity CreateInstance()
        {
            return Activator.CreateInstance(ProxyType) as TEntity;
        }

        public static TEntity CreateInstance(params object[] args)
        {
            return Activator.CreateInstance(ProxyType, args) as TEntity;
        }
    }
}