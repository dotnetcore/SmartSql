using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Abstractions;

namespace SmartSql.Configuration.Tags
{
    public class Placeholder : Tag
    {
        public override TagType Type => TagType.Placeholder;
        public override void BuildSql(RequestContext context)
        {
            if (IsCondition(context))
            {
                Object reqVal = GetPropertyValue(context);
                context.Sql.Append($"{Prepend}{reqVal}");
            }
        }

        public override bool IsCondition(RequestContext context)
        {
            if (context.RequestParameters == null) { return false; }
            return context.RequestParameters.Contains(Property);
        }
    }
}
