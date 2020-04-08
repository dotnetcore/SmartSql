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
            dbSession.BeginTransaction();
            try
            {
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
            dbSession.BeginTransaction(isolationLevel);
            try
            {
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
            dbSession.BeginTransaction();
            try
            {
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
            dbSession.BeginTransaction(isolationLevel);
            try
            {
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
            transaction.BeginTransaction();
            try
            {
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
            transaction.BeginTransaction(isolationLevel);
            try
            {
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
            transaction.BeginTransaction();
            try
            {
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
            transaction.BeginTransaction(isolationLevel);
            try
            {
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
