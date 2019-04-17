using SmartSql.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartSql.Configuration.Tags
{
    public class Dynamic : Tag
    {
        public int? Min { get; set; }
        public override bool IsCondition(AbstractRequestContext context)
        {
            var matched = ChildTags.Sum(childTag => childTag.IsCondition(context) ? 1 : 0);
            if (Min.HasValue)
            {
                if (matched < Min)
                {
                    throw new TagMinMatchedFailException(this, matched);
                }
            }
            return matched > 0;
        }
        public override void BuildSql(AbstractRequestContext context)
        {
            if (IsCondition(context))
            {
                BuildChildSql(context);
            }
        }

        public override void BuildChildSql(AbstractRequestContext context)
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
