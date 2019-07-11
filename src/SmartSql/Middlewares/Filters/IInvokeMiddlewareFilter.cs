using SmartSql.Filters;

namespace SmartSql.Middlewares.Filters
{
    public interface IInvokeMiddlewareFilter : IInvokeFilter, IAsyncInvokeFilter
    {
    }
}