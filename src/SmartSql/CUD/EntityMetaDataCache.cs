using SmartSql.Annotations;
using SmartSql.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SmartSql.CUD
{
    public static class EntityMetaDataCache<TEntity>
    {
        public const string DEFAULT_ID_NAME = "Id";
        public static Type EntityType { get; }
        public static EntityMetaData MetaData { get; }
        public static String TableName => MetaData.TableName;
        public static IDictionary<string, ColumnAttribute> Columns => MetaData.Columns;
        public static ColumnAttribute PrimaryKey
        {
            get
            {
                if (MetaData.PrimaryKey == null)
                {
                    throw new SmartSqlException($"{EntityType.FullName} can not find PrimaryKey.");
                }
                return MetaData.PrimaryKey;
            }
        }
        static EntityMetaDataCache()
        {
            EntityType = typeof(TEntity);
            MetaData = new EntityMetaData();
            InitMetaData();
        }

        private static void InitMetaData()
        {
            InitTableName();
            InitColumns();
        }

        private static void InitColumns()
        {
            MetaData.Columns = EntityType.GetProperties()
                .Where(propInfo => propInfo.GetCustomAttribute<NotMappedAttribute>() == null)
                .ToDictionary((propInfo) =>
                {
                    var column = propInfo.GetCustomAttribute<ColumnAttribute>();
                    return String.IsNullOrEmpty(column?.Name) ? propInfo.Name : column.Name;
                }, (propInfo) =>
                {
                    var column = propInfo.GetCustomAttribute<ColumnAttribute>() ?? new ColumnAttribute(propInfo.Name)
                    {
                        IsPrimaryKey = String.Equals(propInfo.Name, DEFAULT_ID_NAME, StringComparison.CurrentCultureIgnoreCase)
                    };
                    if (String.IsNullOrEmpty(column.Name))
                    {
                        column.Name = propInfo.Name;
                    }
                    column.Property = propInfo;
                    if (column.IsPrimaryKey)
                    {
                        MetaData.PrimaryKey = column;
                    }
                    return column;
                });
        }

        private static void InitTableName()
        {
            MetaData.TableName =
               EntityType.GetCustomAttribute<TableAttribute>(false)?.Name;
            if (String.IsNullOrEmpty(MetaData.TableName))
            {
                MetaData.TableName = EntityType.Name;
            }
        }
    }
}
