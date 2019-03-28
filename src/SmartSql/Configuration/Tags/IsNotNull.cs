using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration.Tags
{
    public class IsNotNull : Tag
    {
        public override bool IsCondition(AbstractRequestContext context)
        {
            return EnsurePropertyValue(context) != null;
        }
    }
}
