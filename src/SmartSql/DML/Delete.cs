using System.Data;

namespace SmartSql.DML
{
    public class Delete : IDML
    {
        public StatementType StatementType => StatementType.Delete;
        public string Operation => "DELETE";
        public string Where => "WHERE";
        public string Table { get; set; }
    }
}