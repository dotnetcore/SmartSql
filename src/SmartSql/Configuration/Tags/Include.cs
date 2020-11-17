using System;

namespace SmartSql.Configuration.Tags
{
    public class Include : Tag
    {
        public String RefId { get; set; }
        public Statement Ref { get; set; }

        public override bool IsCondition(AbstractRequestContext context)
        {
            bool passed = false;

            foreach (var childTag in ChildTags)
            {
                if (childTag.IsCondition(context))
                {
                    passed = true;
                    break;
                }
            }

            if (Required && !passed)
            {
                throw new TagRequiredFailException(this);
            }

            return passed;
        }
    }
}