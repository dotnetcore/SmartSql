using SmartSql.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration.Tags
{
    public class Include : Tag
    {
        public String RefId { get; set; }
        public Statement Ref { get; set; }
        public override bool IsCondition(AbstractRequestContext context)
        {
            if (!Required)
            {
                return true;
            }

            bool passed = false;

            foreach (var childTag in ChildTags)
            {
                if (childTag.IsCondition(context))
                {
                    passed = true;
                    break;
                }
            }
            if (!passed)
            {
                throw new TagRequiredFailException(this);
            }
            return true;
        }
        public override void BuildSql(AbstractRequestContext context)
        {
            if (!IsCondition(context))
            {
                return;
            }
            context.SqlBuilder.Append(" ");
            if (!context.IgnorePrepend)
            {
                context.SqlBuilder.Append(Prepend);
            }
            
            context.SqlBuilder.Append(" ");
            //IncludeTag 特殊处理让IncludeTag的子节点享受和IncludeTag 相同的 IgnorePrepend 待遇
            this.BuildChildSql(context);
        }
    }
}
