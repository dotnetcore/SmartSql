using SmartSql.Abstractions.DbSession;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using SmartSql.Abstractions.DataSource;
using SmartSql.Exceptions;
using Microsoft.Extensions.Logging;

namespace SmartSql.DbSession
{
    public class DbConnectionSession : IDbConnectionSession
    {
        private readonly ILogger _logger;
        public Guid Id { get; private set; }
        public DbProviderFactory DbProviderFactory { get; }
        public IDataSource DataSource { get; }
        public IDbConnection Connection { get; private set; }
        public IDbTransaction Transaction { get; private set; }
        public DbSessionLifeCycle LifeCycle { get; set; }
        public DbConnectionSession(ILoggerFactory loggerFactory, DbProviderFactory dbProviderFactory, IDataSource dataSource)
        {
            _logger = loggerFactory.CreateLogger<DbConnectionSession>();
            Id = Guid.NewGuid();
            LifeCycle = DbSessionLifeCycle.Transient;
            DbProviderFactory = dbProviderFactory;
            DataSource = dataSource;
        }
        public void BeginTransaction()
        {
            OpenConnection();
            _logger.LogDebug("BeginTransaction.");
            Transaction = Connection.BeginTransaction();

            LifeCycle = DbSessionLifeCycle.Scoped;
        }
        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            OpenConnection();
            _logger.LogDebug("BeginTransaction.");
            Transaction = Connection.BeginTransaction(isolationLevel);
            LifeCycle = DbSessionLifeCycle.Scoped;
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
                _logger.LogDebug($"CloseConnection {Connection.GetHashCode()}:{DataSource.Name} ");
                Connection.Dispose();
            }
            Connection = null;
        }

        public void CommitTransaction()
        {
            _logger.LogDebug("CommitTransaction.");
            Transaction.Commit();
            Transaction.Dispose();
            Transaction = null;
            LifeCycle = DbSessionLifeCycle.Transient;
            this.CloseConnection();
        }

        public void Dispose()
        {
            _logger.LogWarning("Dispose.");

            if (Transaction != null)
            {
                if (Connection.State != ConnectionState.Closed)
                {
                    RollbackTransaction();
                }
            }
            else
            {
                CloseConnection();
            }
        }

        public void OpenConnection()
        {
            if (Connection == null)
            {
                CreateConnection();
                try
                {
                    _logger.LogDebug($"OpenConnection {Connection.GetHashCode()} to {DataSource.Name} .");
                    Connection.Open();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"OpenConnection Unable to open connection to { DataSource.Name }.");
                    throw new SmartSqlException($"OpenConnection Unable to open connection to { DataSource.Name }.", ex);
                }
            }
            else if (Connection.State != ConnectionState.Open)
            {
                try
                {
                    _logger.LogDebug($"OpenConnection {Connection.GetHashCode()} to {DataSource.Name} .");
                    Connection.Open();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"OpenConnection Unable to open connection to { DataSource.Name }.");
                    throw new SmartSqlException($"OpenConnection Unable to open connection to { DataSource.Name }.", ex);
                }
            }
        }

        public void RollbackTransaction()
        {
            _logger.LogDebug("RollbackTransaction .");
            Transaction.Rollback();
            Transaction.Dispose();
            Transaction = null;
            LifeCycle = DbSessionLifeCycle.Transient;
            this.CloseConnection();
        }

    }
}
