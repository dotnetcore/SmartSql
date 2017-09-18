using SmartSql.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.SqlMap.Tags
{
    public class Include : Tag
    {
        public override TagType Type => TagType.Include;
        public String RefId { get; set; }
        public Statement Ref { get; set; }
        public override string BuildSql(RequestContext context, string parameterPrefix)
        {
            return Ref.BuildSql(context);
        }

        public override bool IsCondition(object paramObj)
        {
            return true;
        }
    }
}
