using AspectCore.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.AOP
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TransactionAttribute : AbstractInterceptorAttribute
    {
        public string Alias { get; set; } = SmartSqlBuilder.DEFAULT_ALIAS;
        public IsolationLevel Level { get; set; } = IsolationLevel.Unspecified;
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var transaction = SmartSqlContainer.Instance.GetSmartSql(Alias).SqlMapper;
            try
            {
                transaction.BeginTransaction();
                await next.Invoke(context);
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
