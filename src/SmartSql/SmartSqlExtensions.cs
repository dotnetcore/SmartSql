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
    }
}
