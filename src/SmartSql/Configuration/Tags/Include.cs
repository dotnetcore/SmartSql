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
        public override void BuildSql(RequestContext context)
        {
            context.Sql.Append(Prepend);
            Ref.BuildSql(context);
        }
        public override bool IsCondition(RequestContext context)
        {
            return true;
        }
    }
}
