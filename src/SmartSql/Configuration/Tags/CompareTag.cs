using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration.Tags
{
    public abstract class CompareTag<TCompareValue> : Tag
    {
        public TCompareValue CompareValue { get; set; }
    }
}
