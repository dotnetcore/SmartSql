using System.Data;

namespace SmartSql.DML
{
    public class Update : IDML
    {
        public StatementType StatementType => StatementType.Update;
        public string Operation => "UPDATE";
        public string Where => "Where";
        public string Table { get; set; }
    }
}