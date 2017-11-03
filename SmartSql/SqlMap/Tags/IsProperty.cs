using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using SmartSql.Abstractions;
using System.Linq;
namespace SmartSql.SqlMap.Tags
{
    public class IsProperty : Tag
    {
        public override TagType Type => TagType.IsProperty;

        public override bool IsCondition(RequestContext context)
        {
            return !context.RequestParameters.ParameterNames.Contains(Property);
        }
    }
}
