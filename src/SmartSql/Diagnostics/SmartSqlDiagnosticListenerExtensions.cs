using SmartSql.DbSession;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace SmartSql.Diagnostics
{
    public static class SmartSqlDiagnosticListenerExtensions
    {
        public const string SMART_SQL_DIAGNOSTIC_LISTENER = "SmartSqlDiagnosticListener";
        public static readonly DiagnosticListener Instance = new DiagnosticListener(SMART_SQL_DIAGNOSTIC_LISTENER);
        public const string SMART_SQL_PREFIX = "SmartSql.";
        #region Session
        #region Open
        public const string SMART_SQL_BEFORE_DB_SESSION_OPEN = SMART_SQL_PREFIX + nameof(WriteDbSessionOpenBefore);
        public const string SMART_SQL_AFTER_DB_SESSION_OPEN = SMART_SQL_PREFIX + nameof(WriteDbSessionOpenAfter);
        public const string SMART_SQL_ERROR_DB_SESSION_OPEN = SMART_SQL_PREFIX + nameof(WriteDbSessionOpenError);

        public static Guid WriteDbSessionOpenBefore(this DiagnosticListener @this, IDbSession dbSession
            , [CallerMemberName] string operation = "")
        {
            if (!@this.IsEnabled(SMART_SQL_BEFORE_DB_SESSION_OPEN)) return Guid.Empty;
            var operationId = Guid.NewGuid();
            @this.Write(SMART_SQL_BEFORE_DB_SESSION_OPEN, new DbSessionOpenBeforeEventData(operationId, operation)
            {
                DbSession = dbSession
            });
            return operationId;
        }
        public static void WriteDbSessionOpenAfter(this DiagnosticListener @this, Guid operationId, IDbSession dbSession
            , [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(SMART_SQL_AFTER_DB_SESSION_OPEN))
            {
                @this.Write(SMART_SQL_AFTER_DB_SESSION_OPEN, new DbSessionOpenAfterEventData(operationId, operation)
                {
                    DbSession = dbSession
                });
            }
        }
        public static void WriteDbSessionOpenError(this DiagnosticListener @this, Guid operationId, IDbSession dbSession
            , Exception ex, [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(SMART_SQL_ERROR_DB_SESSION_OPEN))
            {
                @this.Write(SMART_SQL_ERROR_DB_SESSION_OPEN, new DbSessionOpenErrorEventData(operationId, operation)
                {
                    DbSession = dbSession,
                    Exception = ex
                });
            }
        }
        #endregion
        #region BeginTransaction
        public const string SMART_SQL_BEFORE_DB_SESSION_BEGINTRANSACTION = SMART_SQL_PREFIX + nameof(WriteDbSessionBeginTransactionBefore);
        public const string SMART_SQL_AFTER_DB_SESSION_BEGINTRANSACTION = SMART_SQL_PREFIX + nameof(WriteDbSessionBeginTransactionAfter);
        public const string SMART_SQL_ERROR_DB_SESSION_BEGINTRANSACTION = SMART_SQL_PREFIX + nameof(WriteDbSessionBeginTransactionError);

        public static Guid WriteDbSessionBeginTransactionBefore(this DiagnosticListener @this, IDbSession dbSession
            , [CallerMemberName] string operation = "")
        {
            if (!@this.IsEnabled(SMART_SQL_BEFORE_DB_SESSION_BEGINTRANSACTION)) return Guid.Empty;
            var operationId = Guid.NewGuid();
            @this.Write(SMART_SQL_BEFORE_DB_SESSION_BEGINTRANSACTION, new DbSessionBeginTransactionBeforeEventData(operationId, operation)
            {
                DbSession = dbSession
            });
            return operationId;
        }
        public static void WriteDbSessionBeginTransactionAfter(this DiagnosticListener @this, Guid operationId, IDbSession dbSession
            , [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(SMART_SQL_AFTER_DB_SESSION_BEGINTRANSACTION))
            {
                @this.Write(SMART_SQL_AFTER_DB_SESSION_BEGINTRANSACTION, new DbSessionBeginTransactionAfterEventData(operationId, operation)
                {
                    DbSession = dbSession
                });
            }
        }
        public static void WriteDbSessionBeginTransactionError(this DiagnosticListener @this, Guid operationId, IDbSession dbSession
            , Exception ex, [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(SMART_SQL_ERROR_DB_SESSION_BEGINTRANSACTION))
            {
                @this.Write(SMART_SQL_ERROR_DB_SESSION_BEGINTRANSACTION, new DbSessionBeginTransactionErrorEventData(operationId, operation)
                {
                    DbSession = dbSession,
                    Exception = ex
                });
            }
        }
        #endregion
        #region Commit
        public const string SMART_SQL_BEFORE_DB_SESSION_COMMIT = SMART_SQL_PREFIX + nameof(WriteDbSessionCommitBefore);
        public const string SMART_SQL_AFTER_DB_SESSION_COMMIT = SMART_SQL_PREFIX + nameof(WriteDbSessionCommitAfter);
        public const string SMART_SQL_ERROR_DB_SESSION_COMMIT = SMART_SQL_PREFIX + nameof(WriteDbSessionCommitError);

        public static Guid WriteDbSessionCommitBefore(this DiagnosticListener @this, IDbSession dbSession
            , [CallerMemberName] string operation = "")
        {
            if (!@this.IsEnabled(SMART_SQL_BEFORE_DB_SESSION_COMMIT)) return Guid.Empty;
            var operationId = Guid.NewGuid();
            @this.Write(SMART_SQL_BEFORE_DB_SESSION_COMMIT, new DbSessionCommitBeforeEventData(operationId, operation)
            {
                DbSession = dbSession
            });
            return operationId;
        }
        public static void WriteDbSessionCommitAfter(this DiagnosticListener @this, Guid operationId, IDbSession dbSession
            , [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(SMART_SQL_AFTER_DB_SESSION_COMMIT))
            {
                @this.Write(SMART_SQL_AFTER_DB_SESSION_COMMIT, new DbSessionCommitAfterEventData(operationId, operation)
                {
                    DbSession = dbSession
                });
            }
        }
        public static void WriteDbSessionCommitError(this DiagnosticListener @this, Guid operationId, IDbSession dbSession
            , Exception ex, [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(SMART_SQL_ERROR_DB_SESSION_COMMIT))
            {
                @this.Write(SMART_SQL_ERROR_DB_SESSION_COMMIT, new DbSessionCommitErrorEventData(operationId, operation)
                {
                    DbSession = dbSession,
                    Exception = ex
                });
            }
        }
        #endregion
        #region Rollback
        public const string SMART_SQL_BEFORE_DB_SESSION_ROLLBACK = SMART_SQL_PREFIX + nameof(WriteDbSessionRollbackBefore);
        public const string SMART_SQL_AFTER_DB_SESSION_ROLLBACK = SMART_SQL_PREFIX + nameof(WriteDbSessionRollbackAfter);
        public const string SMART_SQL_ERROR_DB_SESSION_ROLLBACK = SMART_SQL_PREFIX + nameof(WriteDbSessionRollbackError);

        public static Guid WriteDbSessionRollbackBefore(this DiagnosticListener @this, IDbSession dbSession
            , [CallerMemberName] string operation = "")
        {
            if (!@this.IsEnabled(SMART_SQL_BEFORE_DB_SESSION_ROLLBACK)) return Guid.Empty;
            var operationId = Guid.NewGuid();
            @this.Write(SMART_SQL_BEFORE_DB_SESSION_ROLLBACK, new DbSessionRollbackBeforeEventData(operationId, operation)
            {
                DbSession = dbSession
            });
            return operationId;
        }
        public static void WriteDbSessionRollbackAfter(this DiagnosticListener @this, Guid operationId, IDbSession dbSession
            , [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(SMART_SQL_AFTER_DB_SESSION_ROLLBACK))
            {
                @this.Write(SMART_SQL_AFTER_DB_SESSION_ROLLBACK, new DbSessionRollbackAfterEventData(operationId, operation)
                {
                    DbSession = dbSession
                });
            }
        }
        public static void WriteDbSessionRollbackError(this DiagnosticListener @this, Guid operationId, IDbSession dbSession
            , Exception ex, [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(SMART_SQL_ERROR_DB_SESSION_ROLLBACK))
            {
                @this.Write(SMART_SQL_ERROR_DB_SESSION_ROLLBACK, new DbSessionRollbackErrorEventData(operationId, operation)
                {
                    DbSession = dbSession,
                    Exception = ex
                });
            }
        }
        #endregion
        #region Invoke
        public const string SMART_SQL_BEFORE_DB_SESSION_INVOKE = SMART_SQL_PREFIX + nameof(WriteDbSessionInvokeBefore);
        public const string SMART_SQL_AFTER_DB_SESSION_INVOKE = SMART_SQL_PREFIX + nameof(WriteDbSessionInvokeAfter);
        public const string SMART_SQL_ERROR_DB_SESSION_INVOKE = SMART_SQL_PREFIX + nameof(WriteDbSessionInvokeError);

        public static Guid WriteDbSessionInvokeBefore(this DiagnosticListener @this, ExecutionContext executionContext
            , [CallerMemberName] string operation = "")
        {
            if (!@this.IsEnabled(SMART_SQL_BEFORE_DB_SESSION_INVOKE)) return Guid.Empty;
            var operationId = Guid.NewGuid();
            @this.Write(SMART_SQL_BEFORE_DB_SESSION_INVOKE, new DbSessionInvokeBeforeEventData(operationId, operation)
            {
                DbSession = executionContext.DbSession,
                ExecutionContext = executionContext
            });
            return operationId;
        }
        public static void WriteDbSessionInvokeAfter(this DiagnosticListener @this, Guid operationId, ExecutionContext executionContext
            , [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(SMART_SQL_AFTER_DB_SESSION_INVOKE))
            {
                @this.Write(SMART_SQL_AFTER_DB_SESSION_INVOKE, new DbSessionInvokeAfterEventData(operationId, operation)
                {
                    DbSession = executionContext.DbSession,
                    ExecutionContext = executionContext
                });
            }
        }
        public static void WriteDbSessionInvokeError(this DiagnosticListener @this, Guid operationId, ExecutionContext executionContext
            , Exception ex, [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(SMART_SQL_ERROR_DB_SESSION_INVOKE))
            {
                @this.Write(SMART_SQL_ERROR_DB_SESSION_INVOKE, new DbSessionInvokeErrorEventData(operationId, operation)
                {
                    DbSession = executionContext.DbSession,
                    Exception = ex,
                    ExecutionContext = executionContext
                });
            }
        }
        #endregion
        #region Dispose
        public const string SMART_SQL_BEFORE_DB_SESSION_DISPOSE = SMART_SQL_PREFIX + nameof(WriteDbSessionDisposeBefore);
        public const string SMART_SQL_AFTER_DB_SESSION_DISPOSE = SMART_SQL_PREFIX + nameof(WriteDbSessionDisposeAfter);
        public const string SMART_SQL_ERROR_DB_SESSION_DISPOSE = SMART_SQL_PREFIX + nameof(WriteDbSessionDisposeError);

        public static Guid WriteDbSessionDisposeBefore(this DiagnosticListener @this, IDbSession dbSession
            , [CallerMemberName] string operation = "")
        {
            if (!@this.IsEnabled(SMART_SQL_BEFORE_DB_SESSION_DISPOSE)) return Guid.Empty;
            var operationId = Guid.NewGuid();
            @this.Write(SMART_SQL_BEFORE_DB_SESSION_DISPOSE, new DbSessionDisposeBeforeEventData(operationId, operation)
            {
                DbSession = dbSession
            });
            return operationId;
        }
        public static void WriteDbSessionDisposeAfter(this DiagnosticListener @this, Guid operationId, IDbSession dbSession
            , [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(SMART_SQL_AFTER_DB_SESSION_DISPOSE))
            {
                @this.Write(SMART_SQL_AFTER_DB_SESSION_DISPOSE, new DbSessionDisposeAfterEventData(operationId, operation)
                {
                    DbSession = dbSession
                });
            }
        }
        public static void WriteDbSessionDisposeError(this DiagnosticListener @this, Guid operationId, IDbSession dbSession
            , Exception ex, [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(SMART_SQL_ERROR_DB_SESSION_DISPOSE))
            {
                @this.Write(SMART_SQL_ERROR_DB_SESSION_DISPOSE, new DbSessionDisposeErrorEventData(operationId, operation)
                {
                    DbSession = dbSession,
                    Exception = ex
                });
            }
        }
        #endregion
        #endregion
        #region Initializer
        #endregion
        #region PrepareStatement
        #endregion
        #region Cache
        #endregion
        #region DataSourceFilter
        #endregion
        #region CommandExecuter
        public const string SMART_SQL_BEFORE_COMMAND_EXECUTER_EXECUTE = SMART_SQL_PREFIX + nameof(WriteCommandExecuterExecuteBefore);
        public const string SMART_SQL_AFTER_COMMAND_EXECUTER_EXECUTE = SMART_SQL_PREFIX + nameof(WriteCommandExecuterExecuteAfter);
        public const string SMART_SQL_ERROR_COMMAND_EXECUTER_EXECUTE = SMART_SQL_PREFIX + nameof(WriteCommandExecuterExecuteError);

        public static Guid WriteCommandExecuterExecuteBefore(this DiagnosticListener @this, ExecutionContext executionContext
            , [CallerMemberName] string operation = "")
        {
            if (!@this.IsEnabled(SMART_SQL_BEFORE_COMMAND_EXECUTER_EXECUTE)) return Guid.Empty;
            var operationId = Guid.NewGuid();
            @this.Write(SMART_SQL_BEFORE_COMMAND_EXECUTER_EXECUTE, new CommandExecuterExecuteBeforeEventData(operationId, operation)
            {
                ExecutionContext = executionContext
            });
            return operationId;
        }
        public static void WriteCommandExecuterExecuteAfter(this DiagnosticListener @this, Guid operationId, ExecutionContext executionContext
            , [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(SMART_SQL_AFTER_COMMAND_EXECUTER_EXECUTE))
            {
                @this.Write(SMART_SQL_AFTER_COMMAND_EXECUTER_EXECUTE, new CommandExecuterExecuteAfterEventData(operationId, operation)
                {
                    ExecutionContext = executionContext
                });
            }
        }
        public static void WriteCommandExecuterExecuteError(this DiagnosticListener @this, Guid operationId, ExecutionContext executionContext
            , Exception ex, [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(SMART_SQL_ERROR_COMMAND_EXECUTER_EXECUTE))
            {
                @this.Write(SMART_SQL_ERROR_COMMAND_EXECUTER_EXECUTE, new CommandExecuterExecuteErrorEventData(operationId, operation)
                {
                    Exception = ex,
                    ExecutionContext = executionContext
                });
            }
        }
        #endregion
        #region ResultHandler

        #endregion
    }
}
