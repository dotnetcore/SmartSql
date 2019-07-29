using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using SmartSql.CUD;

namespace SmartSql.Bulk
{
    public static class BulkExtensions
    {
        /// <summary>
        /// List To DataTable
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
                if (columnIndex.Value.GetPropertyValue == null)
                {
                    continue;
                }

                DataColumn dataColumn = new DataColumn(columnIndex.Value.Name, columnIndex.Value.FieldType);
                dataTable.Columns.Add(dataColumn);
            }

            foreach (var entity in list)
            {
                var dataRow = dataTable.NewRow();
                foreach (var columnIndex in EntityMetaDataCache<TEntity>.IndexColumnMaps)
                {
                    if (columnIndex.Value.GetPropertyValue == null)
                    {
                        continue;
                    }
                    var propertyVal = columnIndex.Value.GetPropertyValue(entity);
                    if (columnIndex.Value.Handler != null)
                    {
                        dataRow[columnIndex.Key] = columnIndex.Value.Handler.GetSetParameterValue(propertyVal);
                    }
                    else
                    {
                        dataRow[columnIndex.Key] = propertyVal ?? DBNull.Value;
                    }
                }

                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        public static void Insert<TEntity>(this IBulkInsert bulkInsert, IEnumerable<TEntity> list)
        {
            var dataTable = list.ToDataTable();
            bulkInsert.Table = dataTable;
            bulkInsert.Insert();
        }

        public static async Task InsertAsync<TEntity>(this IBulkInsert bulkInsert, IEnumerable<TEntity> list)
        {
            var dataTable = list.ToDataTable();
            bulkInsert.Table = dataTable;
            await bulkInsert.InsertAsync();
        }
    }
}