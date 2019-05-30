using System.Threading.Tasks;

namespace SmartSql.Middlewares
{
    public class ValidateParameterMiddleware : AbstractMiddleware
    {
        public override void Invoke<TResult>(ExecutionContext executionContext)
        {
            InvokeNext<TResult>(executionContext);
        }

        public override async Task InvokeAsync<TResult>(ExecutionContext executionContext)
        {
            await InvokeNextAsync<TResult>(executionContext);
        }
    }
}