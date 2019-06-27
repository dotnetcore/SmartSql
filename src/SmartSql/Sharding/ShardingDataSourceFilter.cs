using SmartSql.Configuration;
using SmartSql.DataSource;

namespace SmartSql.Sharding
{
    /// <summary>
    /// TODO
    /// </summary>
    public class ShardingDataSourceFilter : IDataSourceFilter, ISetupSmartSql
    {
        private SmartSqlConfig _smartSqlConfig;

        public AbstractDataSource Elect(AbstractRequestContext context)
        {
            throw new System.NotImplementedException();
        }

        public void SetupSmartSql(SmartSqlBuilder smartSqlBuilder)
        {
            _smartSqlConfig = smartSqlBuilder.SmartSqlConfig;
        }
    }
}