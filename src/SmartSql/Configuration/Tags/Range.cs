using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration.Tags
{
    public class Range : Tag
    {
        public Decimal Min { get; set; }
        public Decimal Max { get; set; }
        public override bool IsCondition(RequestContext context)
        {
            if (context.Parameters.TryGetParameterValue<Decimal>(Property, out var paramVal))
            {
                return paramVal > Min && paramVal < Max;
            }
            return false;
        }
    }
}
