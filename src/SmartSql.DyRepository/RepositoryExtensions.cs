using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.DyRepository
{
    public static class RepositoryExtensions
    {
        public static void TransactionWrap(this IRepository repository, Action handler)
        {
            repository.SqlMapper.TransactionWrap(handler);
        }
        public static void TransactionWrap(this IRepository repository, IsolationLevel isolationLevel, Action handler)
        {
            repository.SqlMapper.TransactionWrap(isolationLevel, handler);
        }
        public static async Task TransactionWrapAsync(this IRepository repository, Func<Task> handler)
        {
            await repository.SqlMapper.TransactionWrapAsync(handler);
        }
        public static async Task TransactionWrapAsync(this IRepository repository, IsolationLevel isolationLevel, Func<Task> handler)
        {
            await repository.SqlMapper.TransactionWrapAsync(isolationLevel, handler);
        }
    }
}
