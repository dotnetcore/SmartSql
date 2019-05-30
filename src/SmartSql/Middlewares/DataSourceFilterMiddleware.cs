using SmartSql.Configuration;
using SmartSql.DataSource;
using System.Threading.Tasks;

namespace SmartSql.Middlewares
{
    public class DataSourceFilterMiddleware : AbstractMiddleware
    {
        private readonly IDataSourceFilter _dataSourceFilter;
        public DataSourceFilterMiddleware(SmartSqlConfig smartSqlConfig)
        {
            _dataSourceFilter = smartSqlConfig.DataSourceFilter;
        }
        public override void Invoke<TResult>(ExecutionContext executionContext)
        {
            SetDataSource(executionContext);
            InvokeNext<TResult>(executionContext);
        }
        public override async Task InvokeAsync<TResult>(ExecutionContext executionContext)
        {
            SetDataSource(executionContext);
            await InvokeNextAsync<TResult>(executionContext);
        }
        private void SetDataSource(ExecutionContext executionContext)
        {
            if (executionContext.DbSession.DataSource != null) { return; }
            var dataSource = _dataSourceFilter.Elect(executionContext.Request);
            executionContext.DbSession.SetDataSource(dataSource);
        }
    }
}
