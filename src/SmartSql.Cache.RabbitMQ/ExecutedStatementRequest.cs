using System;

namespace SmartSql.Cache.RabbitMQ
{
    public class ExecutedStatementRequest
    {
        public String FullSqlId { get; set; }
    }
}