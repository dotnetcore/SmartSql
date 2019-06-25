namespace SmartSql.Sharding
{
    public interface IShardingTable
    {
        string ActualTable(AbstractRequestContext requestContext, string logicTableName);
    }
}