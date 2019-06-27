namespace SmartSql.Sharding
{
    public interface IShardingTable : IInitialize
    {
        string ActualTable(AbstractRequestContext requestContext, string logicTableName);
    }
}