using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration.Tags
{
    public class Placeholder : Tag
    {
        public override void BuildSql(AbstractRequestContext context)
        {
            if (IsCondition(context))
            {
                Object reqVal = EnsurePropertyValue(context);
                context.SqlBuilder.Append($"{Prepend}{reqVal}");
            }
        }

        public override bool IsCondition(AbstractRequestContext context)
        {
            return context.Parameters.TryGetValue(Property, out _);
        }
    }
}