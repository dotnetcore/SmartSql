using SmartSql.DbSession;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql
{
    public static class SmartSqlExtensions
    {
        #region IDbSession.TransactionWrap
        public static void TransactionWrap(this IDbSession dbSession, Action handler)
        {
            try
            {
                dbSession.BeginTransaction();
                handler();
                dbSession.CommitTransaction();
            }
            catch (Exception)
            {
                dbSession.RollbackTransaction();
                throw;
            }
        }

        public static void TransactionWrap(this IDbSession dbSession, IsolationLevel isolationLevel, Action handler)
        {
            try
            {
                dbSession.BeginTransaction(isolationLevel);
                handler();
                dbSession.CommitTransaction();
            }
            catch (Exception)
            {
                dbSession.RollbackTransaction();
                throw;
            }
        }

        public static async Task TransactionWrapAsync(this IDbSession dbSession, Func<Task> handler)
        {
            try
            {
                dbSession.BeginTransaction();
                await handler();
                dbSession.CommitTransaction();
            }
            catch (Exception)
            {
                dbSession.RollbackTransaction();
                throw;
            }
        }

        public static async Task TransactionWrapAsync(this IDbSession dbSession, IsolationLevel isolationLevel, Func<Task> handler)
        {
            try
            {
                dbSession.BeginTransaction(isolationLevel);
                await handler();
                dbSession.CommitTransaction();
            }
            catch (Exception)
            {
                dbSession.RollbackTransaction();
                throw;
            }
        }
        #endregion
        #region ITransaction.TransactionWrap
        public static void TransactionWrap(this ITransaction transaction, Action handler)
        {
            try
            {
                transaction.BeginTransaction();
                handler();
                transaction.CommitTransaction();
            }
            catch (Exception)
            {
                transaction.RollbackTransaction();
                throw;
            }
        }

        public static void TransactionWrap(this ITransaction transaction, IsolationLevel isolationLevel, Action handler)
        {
            try
            {
                transaction.BeginTransaction(isolationLevel);
                handler();
                transaction.CommitTransaction();
            }
            catch (Exception)
            {
                transaction.RollbackTransaction();
                throw;
            }
        }

        public static async Task TransactionWrapAsync(this ITransaction transaction, Func<Task> handler)
        {
            try
            {
                transaction.BeginTransaction();
                await handler();
                transaction.CommitTransaction();
            }
            catch (Exception)
            {
                transaction.RollbackTransaction();
                throw;
            }
        }

        public static async Task TransactionWrapAsync(this ITransaction transaction, IsolationLevel isolationLevel, Func<Task> handler)
        {
            try
            {
                transaction.BeginTransaction(isolationLevel);
                await handler();
                transaction.CommitTransaction();
            }
            catch (Exception)
            {
                transaction.RollbackTransaction();
                throw;
            }
        }
        #endregion
    }
}
