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
    }
}
