using System;
using SmartSql.Sharding;

namespace SmartSql.Configuration.Tags
{
    public class LogicTable : ITag
    {
        public Statement Statement { get; set; }
        public ITag Parent { get; set; }
        public String Name { get; set; }
        public IShardingTable ShardingTable { get; }

        public LogicTable(IShardingTable shardingTable)
        {
            ShardingTable = shardingTable;
        }

        public bool IsCondition(AbstractRequestContext context)
        {
            return true;
        }

        public void BuildSql(AbstractRequestContext context)
        {
            var actualTable = ShardingTable.ActualTable(context, Name);
            context.SqlBuilder.Append(actualTable);
        }
    }
}