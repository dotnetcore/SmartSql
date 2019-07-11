namespace SmartSql.Filters
{
    public interface IInvokeFilter : IFilter
    {
        void OnInvoking(ExecutionContext context);

        void OnInvoked(ExecutionContext context);
    }
}