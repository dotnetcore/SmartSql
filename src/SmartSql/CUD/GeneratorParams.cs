using SmartSql.Annotations;
using SmartSql.Configuration;
using SmartSql.Reflection.TypeConstants;
using System;
using System.Collections.Generic;

namespace SmartSql.CUD
{
     public class GeneratorParams
    {
        public SqlMap Map { get; }

        private Type _entityType;

        public GeneratorParams(SqlMap map, Type entityType)
        {
            Map = map;
            _entityType = EntityMetaDataCacheType.MakeGenericType(entityType);
        }

        public string TableName => GetEntityMetaData<string>("TableName");

        public ColumnAttribute PkCol => GetEntityMetaData<ColumnAttribute>("PrimaryKey");

        public SortedDictionary<int, ColumnAttribute> ColumnMaps => GetEntityMetaData<SortedDictionary<int, ColumnAttribute>>("IndexColumnMaps");

        private TData GetEntityMetaData<TData>(string propertyName)
        {
            return (TData)_entityType.GetProperty(propertyName).GetValue(null);
        }
    }
}
