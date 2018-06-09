using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.Utils
{
    public class DataReaderConvert
    {
        public static DataTable ToDataTable(IDataReader dataReader)
        {
            DataTable dataTable = new DataTable();
            InitDataTableColumns(dataReader, dataTable);
            while (dataReader.Read())
            {
                DataRow dataRow = dataTable.NewRow();
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    dataRow[i] = dataReader[i];
                }
                dataTable.Rows.Add(dataRow);
            }
            return dataTable;
        }
        public static async Task<DataTable> ToDataTableAsync(DbDataReader dataReader)
        {
            DataTable dataTable = new DataTable();
            InitDataTableColumns(dataReader, dataTable);
            while (await dataReader.ReadAsync())
            {
                DataRow dataRow = dataTable.NewRow();
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    dataRow[i] = dataReader[i];
                }
                dataTable.Rows.Add(dataRow);
            }
            return dataTable;
        }
        public static DataSet ToDataSet(IDataReader dataReader)
        {
            DataSet dataSet = new DataSet();
            do
            {
                DataTable dataTable = ToDataTable(dataReader);
                dataSet.Tables.Add(dataTable);
            }
            while (dataReader.NextResult());
            return dataSet;
        }
        public static async Task<DataSet> ToDataSetAsync(DbDataReader dataReader)
        {
            DataSet dataSet = new DataSet();
            do
            {
                DataTable dataTable = await ToDataTableAsync(dataReader);
                dataSet.Tables.Add(dataTable);
            }
            while (dataReader.NextResult());
            return dataSet;
        }
        private static void InitDataTableColumns(IDataReader dataReader, DataTable dataTable)
        {
            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                string colName = dataReader.GetName(i);
                Type colType = dataReader.GetFieldType(i);
                DataColumn column = new DataColumn(colName, colType);
                dataTable.Columns.Add(column);
            }
        }
    }
}
