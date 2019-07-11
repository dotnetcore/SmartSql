using System.Threading.Tasks;

namespace SmartSql.Filters
{
    public interface IAsyncInvokeFilter : IFilter
    {
        Task OnInvokingAsync(ExecutionContext context);

        Task OnInvokedAsync(ExecutionContext context);
    }
}