using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration.Tags
{
    public class Range : Tag
    {
        public Decimal Min { get; set; }
        public Decimal Max { get; set; }
        public override bool IsCondition(AbstractRequestContext context)
        {
            var paramVal = EnsurePropertyValue(context);
            decimal numericalVal = Convert.ToDecimal(paramVal);
            return numericalVal >= Min && numericalVal <= Max;
        }
    }
}
