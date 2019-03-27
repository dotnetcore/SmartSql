using System;
using System.Linq;

namespace SmartSql.Configuration.Tags
{
    public class Switch : Tag
    {
        public override bool IsCondition(RequestContext context)
        {
            EnsurePropertyValue(context);
            return true;
        }
        public override void BuildSql(RequestContext context)
        {
            var matchedTag = ChildTags.FirstOrDefault(tag =>
            {
                if (tag is Case caseTag)
                {
                    return caseTag.IsCondition(context);
                }
                return false;
            }) ?? ChildTags.FirstOrDefault(tag => tag is Default);

            matchedTag?.BuildSql(context);

        }

        public class Case : StringCompareTag
        {
            public override bool IsCondition(RequestContext context)
            {
                var reqVal = EnsurePropertyValue(context);
                if (reqVal == null) { return false; }

                var reqValStr = reqVal is Enum ? Convert.ToInt64(reqVal).ToString() : reqVal.ToString();
                return reqValStr.Equals(CompareValue);
            }
        }

        public class Default : Tag
        {
            public override bool IsCondition(RequestContext context)
            {
                return true;
            }
        }
    }
}
