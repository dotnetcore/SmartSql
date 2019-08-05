
using SmartSql.Configuration;

namespace SmartSql.DML
{
    public class Delete : IStatement
    {
        public StatementType StatementType => StatementType.Delete;
        public string Operation => "DELETE";
        public string Where => "WHERE";
        public string Table { get; set; }
    }
}