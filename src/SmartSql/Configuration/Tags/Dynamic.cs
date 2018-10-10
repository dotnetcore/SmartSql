using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Abstractions;

namespace SmartSql.Configuration.Tags
{
    public class Dynamic : Tag
    {
        public override TagType Type => TagType.Dynamic;

        public override bool IsCondition(RequestContext context)
        {
            return true;
        }
        public override void BuildSql(RequestContext context)
        {
            BuildChildSql(context);
        }

        public override void BuildChildSql(RequestContext context)
        {
            if (ChildTags != null && ChildTags.Count > 0)
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
                        context.Sql.Append(" ");
                        context.Sql.Append(Prepend);
                        context.Sql.Append(" ");
                        if (childTag.Type != TagType.SqlText)
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
}
