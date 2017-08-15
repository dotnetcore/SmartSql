using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Abstractions;
using System.Linq;

namespace SmartSql.SqlMap.Tags
{
    public class Where : Dynamic
    {
        public override TagType Type => TagType.Where;
        public override string Prepend { get { return "Where"; } }

        public override string BuildSql(RequestContext context, string parameterPrefix)
        {
            string strSql = BuildChildSql(context, parameterPrefix).ToString();
            if (strSql.Trim() != Prepend)
            {
                return strSql;
            }
            return string.Empty;
        }
    }
}
