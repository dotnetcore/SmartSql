using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration.Tags
{
    public class Env : Tag
    {
        public string DbProvider { get; set; }
        public override bool IsCondition(RequestContext context)
        {
            var dbProvierName = context.ExecutionContext.SmartSqlConfig.Database.DbProvider.Name;
            if (dbProvierName == DbProvider)
            {
                return true;
            }
            return false;
        }
    }
}
