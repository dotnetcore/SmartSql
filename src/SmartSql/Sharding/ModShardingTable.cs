using System.Collections.Generic;

namespace SmartSql.Sharding
{
    public class ModShardingTable : IShardingTable
    {
        
        public string ActualTable(AbstractRequestContext requestContext, string logicTableName)
        {
            throw new System.NotImplementedException();
        }

        public void Initialize(IDictionary<string, object> parameters)
        {
            throw new System.NotImplementedException();
        }
    }
}