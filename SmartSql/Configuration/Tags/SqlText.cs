using SmartSql.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration.Tags
{
    public class SqlText : ITag
    {
        public TagType Type => TagType.SqlText;
        public string BodyText { get; set; }
        public ITag Parent { get; set; }

        public string BuildSql(RequestContext context)
        {
            return BodyText;
        }
    }
}
