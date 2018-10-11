using SmartSql.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.DyRepository
{
    public static class RepositoryExtensions
    {
        public static void SessionWrap(this IRepository repository, Action handler)
        {
            repository.Session.SessionWrap(handler);
        }
        public static void SessionWrap(this IRepository repository, RequestContext context, Action handler)
        {
            repository.Session.SessionWrap(context, handler);
        }
        public static async Task SessionWrapAsync(this IRepository repository, Func<Task> handler)
        {
            await repository.Session.SessionWrapAsync(handler);
        }
        public static async Task SessionWrapAsync(this IRepository repository, RequestContext context, Func<Task> handler)
        {
            await repository.Session.SessionWrapAsync(context, handler);
        }
        public static void TransactionWrap(this IRepository repository, Action handler)
        {
            repository.Transaction.TransactionWrap(handler);
        }
        public static void TransactionWrap(this IRepository repository, IsolationLevel isolationLevel, Action handler)
        {
            repository.Transaction.TransactionWrap(isolationLevel, handler);
        }
        public static async Task TransactionWrapAsync(this IRepository repository, Func<Task> handler)
        {
            await repository.Transaction.TransactionWrapAsync(handler);
        }
        public static async Task TransactionWrapAsync(this IRepository repository, IsolationLevel isolationLevel, Func<Task> handler)
        {
            await repository.Transaction.TransactionWrapAsync(isolationLevel, handler);
        }
    }
}
