using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration.Tags
{
    public class Env : Tag
    {
        public string DbProvider { get; set; }
        public override bool IsCondition(AbstractRequestContext context)
        {
            var dbProvierName = context.ExecutionContext.SmartSqlConfig.Database.DbProvider.Name;
            return dbProvierName == DbProvider;
        }
    }
}
