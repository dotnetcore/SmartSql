using SmartSql.IdGenerator;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration.Tags
{
    public class IdGenerator : ITag
    {
        public IIdGenerator IdGen { get; set; }
        public string Id { get; set; }
        public Statement Statement { get; set; }
        public ITag Parent { get; set; }

        public void BuildSql(RequestContext context)
        {
            var nextId = IdGen.NextId();
            if (!context.Parameters.TryAdd(Id, nextId))
            {
                context.Parameters.TryGetValue(Id, out Data.SqlParameter sqlParameter);
                sqlParameter.Value = nextId;
            }
        }

        public bool IsCondition(RequestContext context)
        {
            return true;
        }
    }
}
