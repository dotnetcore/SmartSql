using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.SqlMap.Tags
{
    public class SqlText : ITag
    {
        public TagType Type => TagType.SqlText;
        public string BodyText { get; set; }
        public string BuildSql(object paramObj, String parameterPrefix)
        {
            return BodyText;
        }
    }
}
