using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.Batch
{
    public static class BatchTableExtensions
    {
        public static DataTable ToDataTable(this DbTable dbTable)
        {
            DataTable dataTable = new DataTable();
            foreach (var col in dbTable.Columns)
            {
                var dataCol = new DataColumn
                {
                    ColumnName = col.Name
                };
                if (col.DataType != null)
                {
                    dataCol.DataType = col.DataType;
                }
                dataTable.Columns.Add(dataCol);
            }
            foreach (var row in dbTable.Rows)
            {
                var dataRow = dataTable.NewRow();
                foreach (var cell in row.Cells)
                {
                    dataRow[cell.Column.Name] = cell.Value;
                }
                dataTable.Rows.Add(dataRow);
            }
            return dataTable;
        }
    }
}
