using SmartSql.Abstractions;
using SmartSql.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration.Tags
{
    public class Set : Dynamic
    {
        public override TagType Type => TagType.Set;
        public override string Prepend { get { return "Set"; } }
    }
}
