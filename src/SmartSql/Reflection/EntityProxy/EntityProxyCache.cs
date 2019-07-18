using System;

namespace SmartSql.Reflection.EntityProxy
{
    public static class EntityProxyCache<TEntity>
    {
        public static Type ProxyType { get; }

        static EntityProxyCache()
        {
            ProxyType = EntityProxyFactory.Instance.CreateProxyType(typeof(TEntity));
        }

        public static TEntity CreateInstance()
        {
            return (TEntity)Activator.CreateInstance(ProxyType);
        }

        public static TEntity CreateInstance(params object[] args)
        {
            return (TEntity)Activator.CreateInstance(ProxyType, args);
        }
    }
}