using SmartSql.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration.Tags
{
    public class IsTrue : Tag
    {
        public override TagType Type => TagType.IsTrue;

        public override bool IsCondition(RequestContext context)
        {
            Object reqVal = GetPropertyValue(context);
            if (reqVal is Boolean)
            {
                return (bool)reqVal == true;
            }
            return false;
        }
    }
}
