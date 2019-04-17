using System;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using SmartSql.Exceptions;

namespace SmartSql.AOP
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class DbSessionAttribute : AbstractInterceptorAttribute
    {
        public string Alias { get; set; } = SmartSqlBuilder.DEFAULT_ALIAS;

        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var sessionStore = context.ServiceProvider.GetSessionStore(Alias);
            if (sessionStore == null)
            {
                throw new SmartSqlException($"can not find SmartSql instance by Alias:{Alias}.");
            }
            if (sessionStore.LocalSession != null)
            {
                await next.Invoke(context); return;
            }

            using (sessionStore)
            {
                sessionStore.Open();
                await next.Invoke(context);
            }
        }
    }
}