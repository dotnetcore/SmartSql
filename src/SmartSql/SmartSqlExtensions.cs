using SmartSql.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql
{
    public static class SmartSqlExtensions
    {
        #region TransactionWrap
        public static void TransactionWrap(this ITransaction transaction, Action handler)
        {
            try
            {
                transaction.BeginTransaction();
                handler();
                transaction.CommitTransaction();
            }
            catch (Exception ex)
            {
                transaction.RollbackTransaction();
                throw ex;
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
            catch (Exception ex)
            {
                transaction.RollbackTransaction();
                throw ex;
            }
        }

        public async static Task TransactionWrapAsync(this ITransaction transaction, Func<Task> handler)
        {
            try
            {
                transaction.BeginTransaction();
                await handler();
                transaction.CommitTransaction();
            }
            catch (Exception ex)
            {
                transaction.RollbackTransaction();
                throw ex;
            }
        }

        public async static Task TransactionWrapAsync(this ITransaction transaction, IsolationLevel isolationLevel, Func<Task> handler)
        {
            try
            {
                transaction.BeginTransaction(isolationLevel);
                await handler();
                transaction.CommitTransaction();
            }
            catch (Exception ex)
            {
                transaction.RollbackTransaction();
                throw ex;
            }
        }
        #endregion
        #region SessionWrap
        public static void SessionWrap(this ISession session, Action handler)
        {
            try
            {
                session.BeginSession();
                handler();
            }
            finally
            {
                session.EndSession();
            }
        }
        public static void SessionWrap(this ISession session, RequestContext context, Action handler)
        {
            try
            {
                session.BeginSession(context);
                handler();
            }
            finally
            {
                session.EndSession();
            }
        }
        public async static Task SessionWrapAsync(this ISession session, Func<Task> handler)
        {
            try
            {
                session.BeginSession();
                await handler();
            }
            finally
            {
                session.EndSession();
            }
        }
        public async static Task SessionWrapAsync(this ISession session, RequestContext context, Func<Task> handler)
        {
            try
            {
                session.BeginSession(context);
                await handler();
            }
            finally
            {
                session.EndSession();
            }
        }
        #endregion
    }
}
