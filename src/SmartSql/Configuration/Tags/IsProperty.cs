using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using SmartSql.Abstractions;
using System.Linq;
namespace SmartSql.Configuration.Tags
{
    public class IsProperty : Tag
    {
        public override TagType Type => TagType.IsProperty;

        public override bool IsCondition(RequestContext context)
        {
            if (context.RequestParameters == null) { return false; }
            return context.RequestParameters.Contains(Property);
        }
    }
}
