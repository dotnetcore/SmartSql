namespace SmartSql.InvokeSync
{
    public static class ExecutionContextExtensions
    {
        public static PublishRequest AsPublishRequest(this ExecutionContext executionContext)
        {
            var reqContext = executionContext.Request;
            return new PublishRequest
            {
                Scope = reqContext.Scope,
                SqlId = reqContext.SqlId,
                RealSql = reqContext.RealSql,
                ReadDb = reqContext.ReadDb,
                DataSourceChoice = reqContext.DataSourceChoice,
                Transaction = reqContext.Transaction,
                IsStatementSql = reqContext.IsStatementSql,
                Parameters = executionContext.Request.Parameters.DbParameters,
                Result = executionContext.Result.GetData()
            };
        }
    }
}