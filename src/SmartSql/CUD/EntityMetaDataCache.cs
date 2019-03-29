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
        /// <summary>
        /// Key :PropertyName
        /// </summary>
        public static SortedDictionary<string, ColumnAttribute> Columns => MetaData.Columns;

        public static SortedDictionary<int, ColumnAttribute> IndexColumnMaps { get; private set; }

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
            var columnIndex = 0;
            var columns = EntityType.GetProperties()
               .Where(propInfo => propInfo.GetCustomAttribute<NotMappedAttribute>() == null)
               .ToDictionary((propInfo) => propInfo.Name,
                   (propInfo) =>
               {
                   var column = propInfo.GetCustomAttribute<ColumnAttribute>() ?? new ColumnAttribute(propInfo.Name)
                   {
                       IsPrimaryKey = String.Equals(propInfo.Name, DEFAULT_ID_NAME, StringComparison.CurrentCultureIgnoreCase)
                   };
                   column.Property = propInfo;
                   if (String.IsNullOrEmpty(column.Name))
                   {
                       column.Name = propInfo.Name;
                   }
                   if (column.FieldType == null)
                   {
                       column.FieldType = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;
                   }
                   if (!column.Ordinal.HasValue)
                   {
                       column.Ordinal = columnIndex;
                   }
                   if (column.IsPrimaryKey)
                   {
                       MetaData.PrimaryKey = column;
                   }
                   columnIndex++;
                   return column;
               });
            MetaData.Columns = new SortedDictionary<string, ColumnAttribute>(columns);
            var indexColMap = MetaData.Columns.ToDictionary((col) =>
            {
                if (!col.Value.Ordinal.HasValue)
                {
                    throw new SmartSqlException($"Entity:{EntityType.FullName} {col.Value.Name}.Ordinal can not be null.");
                }
                return col.Value.Ordinal.Value;
            }, col => col.Value);
            IndexColumnMaps = new SortedDictionary<int, ColumnAttribute>(indexColMap);
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
