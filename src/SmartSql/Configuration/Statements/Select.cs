using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration.Statements
{
    public class Select : Statement
    {
        public override StatementType Type => StatementType.Select;
    }
}
