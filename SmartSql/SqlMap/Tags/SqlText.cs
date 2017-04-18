using SmartSql.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.SqlMap.Tags
{
    public class SqlText : ITag
    {
        public TagType Type => TagType.SqlText;
        public string BodyText { get; set; }
        public string BuildSql(RequestContext context, String parameterPrefix)
        {
            return BodyText;
        }
    }
}
