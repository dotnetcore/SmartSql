using SmartSql.Abstractions;
using SmartSql.Configuration.Statements;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration.Tags
{
    public class Include : Tag
    {
        public override TagType Type => TagType.Include;

        public String RefId { get; set; }
        public Statement Ref { get; set; }
        public override string BuildSql(RequestContext context)
        {
            String sqlStr = Ref.BuildSql(context);
            return $" {Prepend} {sqlStr}";
        }
        public override bool IsCondition(RequestContext context)
        {
            return true;
        }
    }
}
