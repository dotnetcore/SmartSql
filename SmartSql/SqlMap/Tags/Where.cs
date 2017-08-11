using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Abstractions;
using System.Linq;

namespace SmartSql.SqlMap.Tags
{
    public class Where : Tag
    {
        public override TagType Type => TagType.Where;

        public String[] FilterTerms
        {
            get
            {
                return new String[]
                {
                    "And","Or"
                };
            }
        }
        public override bool IsCondition(object paramObj)
        {
            return true;
        }
        public override string BuildSql(RequestContext context, string parameterPrefix)
        {
            StringBuilder strBuilder = BuildChildSql(context, parameterPrefix);
            string strSql = strBuilder.ToString();
            if (!String.IsNullOrWhiteSpace(strSql))
            {
                return $" {Prepend} {strSql}";
            }
            return String.Empty;
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
                        strSql = strSql.TrimStart();
                        string[] sqlTerms = strSql.Split(' ');
                        string firstTerm = sqlTerms[0];
                        if (FilterTerms.Any(term => term.ToUpper() == firstTerm.ToUpper()))
                        {
                            sqlTerms[0] = "";
                            strSql = String.Join("", sqlTerms);
                        }
                        isFirstChild = false;
                    }
                    strBuilder.Append(strSql);
                }
            }
            return strBuilder;
        }
    }
}
