using SmartSql.Abstractions.DbSession;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using SmartSql.Abstractions.DataSource;
using SmartSql.Exceptions;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

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
        public DbConnectionSession(ILogger<DbConnectionSession> logger, DbProviderFactory dbProviderFactory, IDataSource dataSource)
        {
            _logger = logger;
            Id = Guid.NewGuid();
            LifeCycle = DbSessionLifeCycle.Transient;
            DbProviderFactory = dbProviderFactory;
            DataSource = dataSource;
            CreateConnection();
        }
        public void BeginTransaction()
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("BeginTransaction.");
            }
            OpenConnection();
            Transaction = Connection.BeginTransaction();
            LifeCycle = DbSessionLifeCycle.Scoped;
        }
        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("BeginTransaction.");
            }
            OpenConnection();
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
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug($"CloseConnection {Connection.GetHashCode()}:{DataSource.Name} ");
                }
                Connection.Close();
                Connection.Dispose();
            }
            Connection = null;
        }

        public void CommitTransaction()
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("CommitTransaction.");
            }
            if (Transaction == null)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError("Before CommitTransaction,Please BeginTransaction first!");
                }
                throw new SmartSqlException("Before CommitTransaction,Please BeginTransaction first!");
            }
            Transaction.Commit();
            Transaction.Dispose();
            Transaction = null;
            LifeCycle = DbSessionLifeCycle.Transient;
            CloseConnection();
        }

        public void Dispose()
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Dispose.");
            }

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
            if (Connection.State != ConnectionState.Open)
            {
                try
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                    {
                        _logger.LogDebug($"OpenConnection {Connection.GetHashCode()} to {DataSource.Name} .");
                    }
                    Connection.Open();
                }
                catch (Exception ex)
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                    {
                        _logger.LogError($"OpenConnection Unable to open connection to { DataSource.Name }.");
                    }
                    throw new SmartSqlException($"OpenConnection Unable to open connection to { DataSource.Name }.", ex);
                }
            }
        }
        #region Async
        public Task OpenConnectionAsync()
        {
            return OpenConnectionAsync(CancellationToken.None);
        }

        public async Task OpenConnectionAsync(CancellationToken cancellationToken)
        {
            if (Connection.State != ConnectionState.Open)
            {
                try
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                    {
                        _logger.LogDebug($"OpenConnection {Connection.GetHashCode()} to {DataSource.Name} .");
                    }
                    var connAsync = Connection as DbConnection;
                    await connAsync.OpenAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                    {
                        _logger.LogError($"OpenConnection Unable to open connection to { DataSource.Name }.");
                    }
                    throw new SmartSqlException($"OpenConnection Unable to open connection to { DataSource.Name }.", ex);
                }
            }
        }
        #endregion


        public void RollbackTransaction()
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("RollbackTransaction .");
            }
            if (Transaction == null)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError("Before RollbackTransaction,Please BeginTransaction first!");
                }
                throw new SmartSqlException("Before RollbackTransaction,Please BeginTransaction first!");
            }
            try
            {
                Transaction.Rollback();
                Transaction.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(ex.HResult), ex, ex.Message);
                throw ex;
            }
            finally
            {
                Transaction = null;
                LifeCycle = DbSessionLifeCycle.Transient;
                CloseConnection();
            }
        }

        public void Begin()
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Begin .");
            }
            LifeCycle = DbSessionLifeCycle.Scoped;
            OpenConnection();
        }

        public void End()
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("End .");
            }
            LifeCycle = DbSessionLifeCycle.Transient;
            CloseConnection();
        }
    }
}
