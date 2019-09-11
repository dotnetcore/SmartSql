using AspectCore.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using SmartSql.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.AOP
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
    public class TransactionAttribute : AbstractInterceptorAttribute
    {
        public string Alias { get; set; } = SmartSqlBuilder.DEFAULT_ALIAS;
        public IsolationLevel Level { get; set; } = IsolationLevel.Unspecified;
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var sessionStore = context.ServiceProvider.GetSessionStore(this.Alias);
            if (sessionStore == null)
            {
                throw new SmartSqlException($"can not find SmartSql instance by Alias:{this.Alias}.");
            }
            var inTransaction = sessionStore.LocalSession?.Transaction != null;
            if (inTransaction)
            {
                await next.Invoke(context); return;
            }

            using (sessionStore)
            {
                await sessionStore.Open().TransactionWrapAsync(this.Level, async () =>
                {
                    await next.Invoke(context);
                });
            }
        }
    }
}
