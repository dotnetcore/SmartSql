using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration.Tags
{
    public interface ITag
    {
        Statement Statement { get; set; }
        ITag Parent { get; set; }
        bool IsCondition(AbstractRequestContext context);
        void BuildSql(AbstractRequestContext context);
    }


}
