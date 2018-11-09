using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Reflection.Emit;

namespace SmartSql.Batch
{
    public static class BatchTableExtensions
    {
        public static DataTable ToDataTable(this DbTable dbTable, IDictionary<String, ColumnMapping> colMappings)
        {
            DataTable dataTable = new DataTable();
            foreach (var colMappingKV in colMappings)
            {
                var colMapping = colMappingKV.Value;
                var dataCol = new DataColumn
                {
                    ColumnName = colMapping.Mapping
                };
                var dbColumn = dbTable.Columns[colMapping.Column];
                if (dbColumn.DataType != null)
                {
                    dataCol.DataType = dbColumn.DataType;
                }
                if (dbColumn.AutoIncrement != null)
                {
                    dataCol.AutoIncrement = dbColumn.AutoIncrement.Value;
                }
                if (dbColumn.AllowDBNull != null)
                {
                    dataCol.AllowDBNull = dbColumn.AllowDBNull.Value;
                }
                dataTable.Columns.Add(dataCol);
            }
            foreach (var row in dbTable.Rows)
            {
                var dataRow = dataTable.NewRow();
                foreach (var colMappingKV in colMappings)
                {
                    var colMapping = colMappingKV.Value;
                    var dataCol = dataTable.Columns[colMapping.Mapping];
                    if (dataCol.AutoIncrement && !row.Cells.ContainsKey(colMapping.Column))
                    {
                        continue;
                    }
                    dataRow[dataCol] = row[colMapping.Column];
                }
                dataTable.Rows.Add(dataRow);
            }
            return dataTable;
        }

        public static DbTable ToDbTable<T>(this IEnumerable<T> list)
            where T : class
        {
            throw new NotImplementedException();
        }
    }
}
