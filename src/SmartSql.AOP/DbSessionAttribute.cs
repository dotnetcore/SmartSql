using AspectCore.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.AOP
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class DbSessionAttribute : AbstractInterceptorAttribute
    {
        public string Alias { get; set; } = Consts.DEFAULT_SMARTSQL_CONFIG_PATH;

        public override Task Invoke(AspectContext context, AspectDelegate next)
        {
            var dbSession = MapperContainer.Instance.GetSqlMapperByAlias(Alias);
            try
            {
                dbSession.BeginSession();
                return next.Invoke(context);
            }
            finally
            {
                dbSession.EndSession();
            }
        }
    }
}
