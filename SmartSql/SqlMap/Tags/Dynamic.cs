using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Abstractions;

namespace SmartSql.SqlMap.Tags
{
    public class Dynamic : Tag
    {
        public override TagType Type => throw new NotImplementedException();

        public override bool IsCondition(object paramObj)
        {
            return true;
        }
        public override string BuildSql(RequestContext context, string parameterPrefix)
        {
            return BuildChildSql(context, parameterPrefix).ToString();
        }

        public override StringBuilder BuildChildSql(RequestContext context, string parameterPrefix)
        {
            StringBuilder strBuilder = new StringBuilder();
            if (ChildTags != null && ChildTags.Count > 0)
            {
                bool isFirstChild = true;
                foreach (var childTag in ChildTags)
                {
                    string strSql = childTag.BuildSql(context, parameterPrefix);
                    if (String.IsNullOrWhiteSpace(strSql))
                    {
                        continue;
                    }
                    if (isFirstChild)
                    {
                        if (!(childTag is SqlText))
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
