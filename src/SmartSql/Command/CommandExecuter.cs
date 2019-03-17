using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartSql.Command
{
    public class CommandExecuter : ICommandExecuter
    {
        public DataTable GetDateTable(ExecutionContext executionContext)
        {
            DataTable dataTable = new DataTable();
            executionContext.DbSession.Open();
            DbCommand dbCmd = CreateCmd(executionContext);
            var dataAdapter = executionContext.SmartSqlConfig.Database.DbProvider.Factory.CreateDataAdapter();
            dataAdapter.SelectCommand = dbCmd;
            dataAdapter.Fill(dataTable);
            return dataTable;
        }

        public DataSet GetDateSet(ExecutionContext executionContext)
        {
            DataSet dataSet = new DataSet();
            executionContext.DbSession.Open();
            DbCommand dbCmd = CreateCmd(executionContext);
            var dataAdapter = executionContext.SmartSqlConfig.Database.DbProvider.Factory.CreateDataAdapter();
            dataAdapter.SelectCommand = dbCmd;
            dataAdapter.Fill(dataSet);
            return dataSet;
        }
        public int ExecuteNonQuery(ExecutionContext executionContext)
        {
            executionContext.DbSession.Open();
            DbCommand dbCmd = CreateCmd(executionContext);
            return dbCmd.ExecuteNonQuery();
        }
        private DbCommand CreateCmd(ExecutionContext executionContext)
        {
            var dbSession = executionContext.DbSession;
            var dbCmd = dbSession.Connection.CreateCommand();
            dbCmd.CommandType = executionContext.Request.CommandType;
            dbCmd.Transaction = dbSession.Transaction;
            dbCmd.CommandText = executionContext.Request.RealSql;
            foreach (var dbParam in executionContext.Request.Parameters.DbParameters.Values)
            {
                dbCmd.Parameters.Add(dbParam);
            }
            return dbCmd;
        }

        public async Task<int> ExecuteNonQueryAsync(ExecutionContext executionContext)
        {
            await executionContext.DbSession.OpenAsync();
            DbCommand dbCmd = CreateCmd(executionContext);
            return await dbCmd.ExecuteNonQueryAsync();
        }

        public async Task<int> ExecuteNonQueryAsync(ExecutionContext executionContext, CancellationToken cancellationToken)
        {
            await executionContext.DbSession.OpenAsync(cancellationToken);
            DbCommand dbCmd = CreateCmd(executionContext);
            return await dbCmd.ExecuteNonQueryAsync(cancellationToken);
        }

        public DataReaderWrapper ExecuteReader(ExecutionContext executionContext)
        {
            executionContext.DbSession.Open();
            DbCommand dbCmd = CreateCmd(executionContext);
            var dbReader = dbCmd.ExecuteReader();
            return new DataReaderWrapper(dbReader);
        }

        public async Task<DataReaderWrapper> ExecuteReaderAsync(ExecutionContext executionContext)
        {
            await executionContext.DbSession.OpenAsync();
            DbCommand dbCmd = CreateCmd(executionContext);
            var dbReader = await dbCmd.ExecuteReaderAsync();
            return new DataReaderWrapper(dbReader);
        }

        public async Task<DataReaderWrapper> ExecuteReaderAsync(ExecutionContext executionContext, CancellationToken cancellationToken)
        {
            await executionContext.DbSession.OpenAsync(cancellationToken);
            DbCommand dbCmd = CreateCmd(executionContext);
            var dbReader = await dbCmd.ExecuteReaderAsync(cancellationToken);
            return new DataReaderWrapper(dbReader);
        }

        public object ExecuteScalar(ExecutionContext executionContext)
        {
            executionContext.DbSession.Open();
            DbCommand dbCmd = CreateCmd(executionContext);
            return dbCmd.ExecuteScalar();
        }

        public async Task<object> ExecuteScalarAsync(ExecutionContext executionContext)
        {
            await executionContext.DbSession.OpenAsync();
            DbCommand dbCmd = CreateCmd(executionContext);
            return await dbCmd.ExecuteScalarAsync();
        }

        public async Task<object> ExecuteScalarAsync(ExecutionContext executionContext, CancellationToken cancellationToken)
        {
            await executionContext.DbSession.OpenAsync(cancellationToken);
            DbCommand dbCmd = CreateCmd(executionContext);
            return await dbCmd.ExecuteScalarAsync(cancellationToken);
        }

        public async Task<DataTable> GetDateTableAsync(ExecutionContext executionContext)
        {
            DataTable dataTable = new DataTable();
            await executionContext.DbSession.OpenAsync();
            DbCommand dbCmd = CreateCmd(executionContext);
            var dataAdapter = executionContext.SmartSqlConfig.Database.DbProvider.Factory.CreateDataAdapter();
            dataAdapter.SelectCommand = dbCmd;
            dataAdapter.Fill(dataTable);
            return dataTable;
        }

        public async Task<DataSet> GetDateSetAsync(ExecutionContext executionContext)
        {
            DataSet dataSet = new DataSet();
            await executionContext.DbSession.OpenAsync();
            DbCommand dbCmd = CreateCmd(executionContext);
            var dataAdapter = executionContext.SmartSqlConfig.Database.DbProvider.Factory.CreateDataAdapter();
            dataAdapter.SelectCommand = dbCmd;
            dataAdapter.Fill(dataSet);
            return dataSet;
        }
    }
}
