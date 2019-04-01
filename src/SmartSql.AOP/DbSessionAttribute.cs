using System;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using SmartSql;
namespace SmartSql.AOP
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class DbSessionAttribute : AbstractInterceptorAttribute
    {
        public string Alias { get; set; } = SmartSqlBuilder.DEFAULT_ALIAS;

        public override Task Invoke(AspectContext context, AspectDelegate next)
        {
            using (SmartSqlContainer.Instance.GetSmartSql(Alias).SmartSqlConfig.SessionStore.Open())
            {
                return next.Invoke(context);
            }
        }
    }
}