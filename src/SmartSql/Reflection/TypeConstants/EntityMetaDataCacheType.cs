using System;
using System.Reflection;
using SmartSql.CUD;

namespace SmartSql.Reflection.TypeConstants
{
    public class EntityMetaDataCacheType
    {
        public static readonly Type GenericType = typeof(EntityMetaDataCache<>);

        public static Type MakeGenericType(Type entityType)
        {
            return GenericType.MakeGenericType(entityType);
        }

        public static void InitTypeHandler(Type entityType)
        {
            GenericType.MakeGenericType(entityType).GetMethod("InitTypeHandler").Invoke(null, null);
        }

        public static string GetTableName(Type entityType)
        {
            return GetEntityMetaData<string>(entityType, "TableName");
        }

        public static TData GetEntityMetaData<TData>(Type entityType, string propertyName)
        {
            return (TData)GenericType.MakeGenericType(entityType).GetProperty(propertyName).GetValue(null);
        }
    }
}