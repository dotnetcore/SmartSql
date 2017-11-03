using SmartSql.Abstractions;
using SmartSql.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.SqlMap.Tags
{
    public class IsNull : Tag
    {
        public override TagType Type => TagType.IsNull;

        public override bool IsCondition(RequestContext context)
        {
            Object reqVal = GetValue(context);
            return reqVal == null;
        }
    }
}
