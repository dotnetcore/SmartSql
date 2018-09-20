using AspectCore.DynamicProxy;
using System;
using System.Threading.Tasks;
using SmartSql;
using System.Data;
using SmartSql.Exceptions;

namespace SmartSql.AOP
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TransactionAttribute : AbstractInterceptorAttribute
    {
        public string Alias { get; set; } = Consts.DEFAULT_SMARTSQL_CONFIG_PATH;
        public IsolationLevel Level { get; set; } = IsolationLevel.Unspecified;
        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            var transaction = MapperContainer.Instance.GetSqlMapperByAlias(Alias);
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
