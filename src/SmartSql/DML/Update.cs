
using SmartSql.Configuration;

namespace SmartSql.DML
{
    public class Update : IStatement
    {
        public StatementType StatementType => StatementType.Update;
        public string Operation => "UPDATE";
        public string Where => "Where";
        public string Table { get; set; }
    }
}