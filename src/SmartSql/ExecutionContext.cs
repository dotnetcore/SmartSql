using SmartSql.Configuration;
using SmartSql.Data;
using SmartSql.DbSession;
using System.Collections.Generic;

namespace SmartSql
{
    public class ExecutionContext
    {
        public ExecutionType Type => Request.ExecutionType;
        public SmartSqlConfig SmartSqlConfig { get; set; }
        public IDbSession DbSession { get; set; }
        public AbstractRequestContext Request { get; set; }
        public DataReaderWrapper DataReaderWrapper { get; set; }
        public ResultContext Result { get; set; }
    }

    public enum ExecutionType
    {
        Execute = 1,
        ExecuteScalar = 2,
        Query = 3,
        QuerySingle = 4,
        GetDataTable=5,
        GetDataSet=6
    }
}
