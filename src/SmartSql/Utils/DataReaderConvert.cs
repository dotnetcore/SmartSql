using SmartSql.Abstractions;
using SmartSql.Abstractions.DataReaderDeserializer;
using SmartSql.Configuration.Maps;
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
        #region DataTable
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
        public static async Task<DataTable> ToDataTableAsync(IDataReaderWrapper dataReader)
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
        public static async Task<DataSet> ToDataSetAsync(IDataReaderWrapper dataReader)
        {
            DataSet dataSet = new DataSet();
            do
            {
                DataTable dataTable = await ToDataTableAsync(dataReader);
                dataSet.Tables.Add(dataTable);
            }
            while (await dataReader.NextResultAsync());
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
        #endregion
        #region DbTable
        public static DbTable ToDbTable(IDataReader dataReader)
        {
            DbTable dbTable = new DbTable();
            InitDbTableColumns(dataReader, dbTable);
            while (dataReader.Read())
            {
                var dbRow = dbTable.AddRow();
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    var colName = dataReader.GetName(i);
                    dbRow[colName] = dataReader[colName];
                }
            }
            return dbTable;
        }
        public static DbSet ToDbSet(IDataReader dataReader)
        {
            DbSet dbSet = new DbSet();
            do
            {
                var dbTable = ToDbTable(dataReader);
                dbSet.Tables.Add(dbTable);
            }
            while (dataReader.NextResult());
            return dbSet;
        }
        public async static Task<DbTable> ToDbTableAsync(IDataReaderWrapper dataReader)
        {
            DbTable dbTable = new DbTable();
            InitDbTableColumns(dataReader, dbTable);
            while (await dataReader.ReadAsync())
            {
                var dbRow = dbTable.AddRow();
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    var colName = dataReader.GetName(i);
                    dbRow[colName] = dataReader[colName];
                }
            }
            return dbTable;
        }
        public async static Task<DbSet> ToDbSetAsync(IDataReaderWrapper dataReader)
        {
            DbSet dbSet = new DbSet();
            do
            {
                var dbTable = await ToDbTableAsync(dataReader);
                dbSet.Tables.Add(dbTable);
            }
            while (await dataReader.NextResultAsync());
            return dbSet;
        }

        private static void InitDbTableColumns(IDataReader dataReader, DbTable dbTable)
        {
            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                string colName = dataReader.GetName(i);
                Type colType = dataReader.GetFieldType(i);
                dbTable.AddColumn(colName, colType);
            }
        }
        #endregion
    }
}
