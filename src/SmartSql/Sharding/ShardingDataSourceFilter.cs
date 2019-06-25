using SmartSql.DataSource;

namespace SmartSql.Sharding
{
    public class ShardingDataSourceFilter : IDataSourceFilter
    {
        public AbstractDataSource Elect(AbstractRequestContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}