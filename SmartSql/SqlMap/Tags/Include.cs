using SmartSql.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.SqlMap.Tags
{
    public class Include : ITag
    {
        public TagType Type => TagType.Include;
        public String RefId { get; set; }
        public Statement Ref { get; set; }
        public string BuildSql(RequestContext context)
        {
            return Ref.BuildSql(context);
        }


    }
}
