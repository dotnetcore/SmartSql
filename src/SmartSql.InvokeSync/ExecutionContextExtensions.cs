using System.Linq;

namespace SmartSql.InvokeSync
{
    public static class ExecutionContextExtensions
    {
        public static SyncRequest AsPublishRequest(this ExecutionContext executionContext)
        {
            var reqContext = executionContext.Request;
            return new SyncRequest
            {
                CommandType = reqContext.CommandType,
                Scope = reqContext.Scope,
                SqlId = reqContext.SqlId,
                RealSql = reqContext.RealSql,
                ReadDb = reqContext.ReadDb,
                DataSourceChoice = reqContext.DataSourceChoice,
                Transaction = reqContext.Transaction,
                IsStatementSql = reqContext.IsStatementSql,
                Parameters =
                    reqContext.Parameters.DbParameters.ToDictionary(keyVal => keyVal.Key, keyVal => keyVal.Value.Value),
                Result = executionContext.Result.GetData()
            };
        }
    }
}