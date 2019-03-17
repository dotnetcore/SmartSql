using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration.Tags
{
    public class Placeholder : Tag
    {
        public override void BuildSql(RequestContext context)
        {
            if (IsCondition(context))
            {
                Object reqVal = GetPropertyValue(context);
                context.SqlBuilder.Append($"{Prepend}{reqVal}");
            }
        }

        public override bool IsCondition(RequestContext context)
        {
            return context.Parameters.ContainsKey(Property);
        }
    }
}
