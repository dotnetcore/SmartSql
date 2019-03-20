using SmartSql.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartSql.Configuration.Tags
{
    public class Dynamic : Tag
    {
        public override bool IsCondition(RequestContext context)
        {
            var passed = ChildTags.Any(childTag => childTag.IsCondition(context));
            if (Required && !passed)
            {
                throw new TagRequiredFailException(this);
            }
            return passed;
        }
        public override void BuildSql(RequestContext context)
        {
            BuildChildSql(context);
        }

        public override void BuildChildSql(RequestContext context)
        {
            bool isFirstChild = true;
            foreach (var childTag in ChildTags)
            {
                if (!childTag.IsCondition(context))
                {
                    continue;
                }
                if (isFirstChild)
                {
                    isFirstChild = false;
                    context.SqlBuilder.Append(" ");
                    context.SqlBuilder.Append(Prepend);
                    context.SqlBuilder.Append(" ");
                    if (!(childTag is SqlText))
                    {
                        context.IgnorePrepend = true;
                    }
                    childTag.BuildSql(context);
                }
                else
                {
                    childTag.BuildSql(context);
                }
            }
        }
    }
}
