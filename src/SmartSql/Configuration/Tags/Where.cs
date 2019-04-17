using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SmartSql.Configuration.Tags
{
    public class Where : Dynamic
    {
        public override string Prepend => "Where";
        public override bool IsCondition(AbstractRequestContext context)
        {
            bool passed = false;
            var matched = ChildTags.Sum(childTag =>
            {
                if (childTag is SqlText)
                {
                    passed = true;
                    return 0;
                }
                return childTag.IsCondition(context) ? 1 : 0;
            });
            if (Min.HasValue)
            {
                if (matched < Min)
                {
                    throw new TagMinMatchedFailException(this, matched);
                }
            }
            return passed || matched > 0;
        }
    }
}
