using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration.Statements
{
    public class Insert : Statement
    {
        public override StatementType Type => StatementType.Insert;
        //public bool ReturnPrimaryKey { get; set; }
    }
}
