using System;
using System.Data;

namespace SmartSql.DyRepository
{
    public class StatementAttribute : Attribute
    {
        public string Scope { get; set; }
        public string Id { get; set; }
        public ExecuteBehavior Execute { get; set; } = ExecuteBehavior.Auto;
        public string Sql { get; set; }
    }

    public enum ExecuteBehavior
    {
        Auto = 0,
        Execute = 1,
        ExecuteScalar = 2,
        Query = 3,
        QuerySingle = 4,
        GetDataTable = 5,
        GetDataSet = 6
    }
}