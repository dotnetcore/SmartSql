using SmartSql.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration.Tags
{
    public class Env : Tag
    {
        public override TagType Type => TagType.Env;
        public string DbProvider { get; set; }
        public override bool IsCondition(RequestContext context)
        {
            var dbProvierName = context.SmartSqlContext.DbProvider.Name;
            if (dbProvierName == DbProvider)
            {
                return true;
            }
            return false;
        }
    }
}
