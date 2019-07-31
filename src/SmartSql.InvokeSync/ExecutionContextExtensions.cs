using System;
using System.Linq;

namespace SmartSql.InvokeSync
{
    public static class ExecutionContextExtensions
    {
        public static SyncRequest AsSyncRequest(this ExecutionContext executionContext)
        {
            var reqContext = executionContext.Request;
            
            return new SyncRequest
            {
                Id = Guid.NewGuid(),
                DbSessionId = executionContext.DbSession.Id,
                CommandType = reqContext.CommandType,
                ParameterPrefix = executionContext.SmartSqlConfig.Database.DbProvider.ParameterPrefix,
                StatementType = reqContext.Statement?.StatementType,
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