namespace SmartSql.InvokeSync
{
    public interface ISyncFilter
    {
        bool Filter(ExecutionContext executionContext);
    }
}