using System;
using System.Linq;

namespace SmartSql.Configuration.Tags
{
    public class Switch : Tag
    {
        public override bool IsCondition(AbstractRequestContext context)
        {
            EnsurePropertyValue(context);
            var matchedTag = GetMatchedTag(context);
            return matchedTag != null;
        }

        public override void BuildSql(AbstractRequestContext context)
        {
            var matchedTag = GetMatchedTag(context);
            matchedTag?.BuildSql(context);
        }

        private ITag GetMatchedTag(AbstractRequestContext context)
        {
            var matchedTag = ChildTags.FirstOrDefault(tag =>
            {
                if (tag is Case caseTag)
                {
                    return caseTag.IsCondition(context);
                }

                return false;
            }) ?? ChildTags.FirstOrDefault(tag => tag is Default);
            return matchedTag;
        }

        public class Case : StringCompareTag
        {
            public override bool IsCondition(AbstractRequestContext context)
            {
                var reqVal = EnsurePropertyValue(context);
                if (reqVal == null)
                {
                    return false;
                }

                var reqValStr = reqVal is Enum ? Convert.ToInt64(reqVal).ToString() : reqVal.ToString();
                return reqValStr.Equals(CompareValue);
            }
        }

        public class Default : Tag
        {
            public override bool IsCondition(AbstractRequestContext context)
            {
                return true;
            }
        }
    }
}