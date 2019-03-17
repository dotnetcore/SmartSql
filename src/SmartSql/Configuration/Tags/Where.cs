using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SmartSql.Configuration.Tags
{
    public class Where : Dynamic
    {
        public override string Prepend { get { return "Where"; } }
    }
}
