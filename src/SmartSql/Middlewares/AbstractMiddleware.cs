using System.Threading.Tasks;

namespace SmartSql.Middlewares
{
    public abstract class AbstractMiddleware : IMiddleware
    {
        public IMiddleware Next { get; set; }

        public abstract void Invoke<TResult>(ExecutionContext executionContext);

        protected void InvokeNext<TResult>(ExecutionContext executionContext)
        {
            Next?.Invoke<TResult>(executionContext);
        }

        public abstract Task InvokeAsync<TResult>(ExecutionContext executionContext);

        protected async Task InvokeNextAsync<TResult>(ExecutionContext executionContext)
        {
            if (Next != null)
            {
                await Next.InvokeAsync<TResult>(executionContext);
            }
        }
    }
}