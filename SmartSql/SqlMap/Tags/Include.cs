using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.SqlMap.Tags
{
    public class Include : ITag
    {
        public TagType Type => TagType.Include;

        public string BodyText { get; set; }
        public String RefId { get; set; }
        public Statement Ref { get; set; }
        public string BuildSql(object paramObj, string parameterPrefix)
        {
            return Ref.BuildSql(paramObj);
        }
    }
}
