using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Abstractions;
using System.Linq;

namespace SmartSql.Configuration.Tags
{
    public class Where : Dynamic
    {
        public override TagType Type => TagType.Where;
        public override string Prepend { get { return "Where"; } }
    }
}
