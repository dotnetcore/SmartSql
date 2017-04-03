using SmartSql.Abstractions.DbSession;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using SmartSql.Abstractions.DataSource;
using SmartSql.Exceptions;
using SmartSql.Abstractions.Logging;

namespace SmartSql.DbSession
{
    public class DbConnectionSession : IDbConnectionSession
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(DbConnectionSession));
        public DbProviderFactory DbProviderFactory { get; }
        public IDataSource DataSource { get; }
        public IDbConnection Connection { get; private set; }
        public IDbTransaction Transaction { get; private set; }
        public bool IsTransactionOpen { get; private set; }
        public DbConnectionSession(DbProviderFactory dbProviderFactory, IDataSource dataSource)
        {
            DbProviderFactory = dbProviderFactory;
            DataSource = dataSource;
        }
        public void BeginTransaction()
        {
            if (Connection == null || Connection.State != ConnectionState.Open)
            {
                OpenConnection();
            }
            _logger.Debug("DbConnectionSession.BeginTransaction.");
            Transaction = Connection.BeginTransaction();

            IsTransactionOpen = true;
        }
        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            if (Connection == null || Connection.State != ConnectionState.Open)
            {
                OpenConnection();
            }
            _logger.Debug("DbConnectionSession.BeginTransaction.");
            Transaction = Connection.BeginTransaction(isolationLevel);
            IsTransactionOpen = true;
        }
        public void CreateConnection()
        {
            Connection = DbProviderFactory.CreateConnection();
            Connection.ConnectionString = DataSource.ConnectionString;
        }

        public void CloseConnection()
        {
            if ((Connection != null) && (Connection.State != ConnectionState.Closed))
            {
                Connection.Close();
                _logger.Debug($"DbConnectionSession.CloseConnection {Connection.GetHashCode()}:{DataSource.Name} ");
                Connection.Dispose();
            }
            Connection = null;
        }

        public void CommitTransaction()
        {
            _logger.Debug("DbConnectionSession.CommitTransaction.");
            Transaction.Commit();
            Transaction.Dispose();
            Transaction = null;
            IsTransactionOpen = false;

            if (Connection.State != ConnectionState.Closed)
            {
                this.CloseConnection();
            }
        }

        public void Dispose()
        {
            _logger.Debug("DbConnectionSession.Dispose.");
            if (IsTransactionOpen)
            {
                if (Connection.State != ConnectionState.Closed)
                {
                    RollbackTransaction();
                    IsTransactionOpen = false;
                }
            }
            else
            {
                if (Connection.State != ConnectionState.Closed)
                {
                    CloseConnection();
                }
            }
        }

        public void OpenConnection()
        {
            if (Connection == null)
            {
                CreateConnection();
                try
                {
                    _logger.Debug($"DbConnectionSession.OpenConnection {Connection.GetHashCode()} to {DataSource.Name} .");
                    Connection.Open();
                }
                catch (Exception ex)
                {
                    _logger.Error($"DbConnectionSession.OpenConnection Unable to open connection to { DataSource.Name }.");
                    throw new SmartSqlException($"DbConnectionSession.OpenConnection Unable to open connection to { DataSource.Name }.", ex);
                }
            }
            else if (Connection.State != ConnectionState.Open)
            {
                try
                {
                    _logger.Debug($"DbConnectionSession.OpenConnection {Connection.GetHashCode()} to {DataSource.Name} .");
                    Connection.Open();
                }
                catch (Exception ex)
                {
                    _logger.Error($"DbConnectionSession.OpenConnection Unable to open connection to { DataSource.Name }.");
                    throw new SmartSqlException($"DbConnectionSession.OpenConnection Unable to open connection to { DataSource.Name }.", ex);
                }
            }
        }

        public void RollbackTransaction()
        {
            _logger.Debug("DbConnectionSession.RollbackTransaction .");
            Transaction.Rollback();
            Transaction.Dispose();
            Transaction = null;
            IsTransactionOpen = false;
            if (Connection.State != ConnectionState.Closed)
            {
                this.CloseConnection();
            }
        }

    }
}
