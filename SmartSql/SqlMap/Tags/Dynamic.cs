using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Abstractions;

namespace SmartSql.SqlMap.Tags
{
    public class Dynamic : Tag
    {
        public override TagType Type => TagType.Dynamic;

        public override bool IsCondition(RequestContext context)
        {
            return true;
        }
        public override string BuildSql(RequestContext context)
        {
            return BuildChildSql(context).ToString();
        }

        public override StringBuilder BuildChildSql(RequestContext context)
        {
            StringBuilder strBuilder = new StringBuilder();
            if (ChildTags != null && ChildTags.Count > 0)
            {
                bool isFirstChild = true;
                foreach (var childTag in ChildTags)
                {
                    string strSql = childTag.BuildSql(context);
                    if (String.IsNullOrWhiteSpace(strSql))
                    {
                        continue;
                    }
                    if (isFirstChild)
                    {
                        if (childTag is Tag)
                        {
                            Tag tag = childTag as Tag;
                            strSql = strSql.TrimStart();
                            if (!String.IsNullOrWhiteSpace(tag.Prepend))
                            {
                                string prepend = tag.Prepend.TrimStart();
                                strSql = strSql.TrimStart(prepend.ToCharArray());
                            }
                        }
                        strSql = $" {Prepend} {strSql}";
                        isFirstChild = false;
                    }

                    strBuilder.Append(strSql);
                }
            }
            return strBuilder;
        }
    }
}
