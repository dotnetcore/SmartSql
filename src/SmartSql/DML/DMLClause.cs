using System;
using System.Collections.Generic;
using System.Data;

namespace SmartSql.DML
{
    public class DMLClause
    {
        public IList<IStatement> Statements { get; set; }
    }
}