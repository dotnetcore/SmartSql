using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SmartSql.CUD;

namespace SmartSql.Bulk
{
    public static class BulkExtensions
    {
        /// <summary>
        /// List To DataTable
        /// 待优化 -> IL
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<TEntity>(this IEnumerable<TEntity> list)
        {
            var tableName = EntityMetaDataCache<TEntity>.TableName;
            var dataTable = new DataTable(tableName);
            foreach (var columnIndex in EntityMetaDataCache<TEntity>.IndexColumnMaps)
            {
                dataTable.Columns.Add(columnIndex.Value.Name, columnIndex.Value.FieldType);
            }
            foreach (var entity in list)
            {
                var dataRow = dataTable.NewRow();
                foreach (var columnIndex in EntityMetaDataCache<TEntity>.IndexColumnMaps)
                {
                    var propertyVal = columnIndex.Value.Property.GetValue(entity, null);
                    dataRow[columnIndex.Key] = propertyVal ?? DBNull.Value;
                }
                dataTable.Rows.Add(dataRow);
            }
            return dataTable;
        }
    }
}
