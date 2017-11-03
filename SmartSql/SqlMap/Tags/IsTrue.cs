using SmartSql.Abstractions;
using SmartSql.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.SqlMap.Tags
{
    public class IsTrue : Tag
    {
        public override TagType Type => TagType.IsTrue;

        public override bool IsCondition(RequestContext context)
        {
            Object reqVal = GetValue(context);
            if (reqVal is Boolean)
            {
                return (bool)reqVal == true;
            }
            return false;
        }
    }
}
