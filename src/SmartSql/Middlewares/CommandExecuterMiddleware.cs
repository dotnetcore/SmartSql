using SmartSql.Command;
using SmartSql.Configuration;
using System;
using System.Data;
using System.Threading.Tasks;

namespace SmartSql.Middlewares
{
    public class CommandExecuterMiddleware : IMiddleware
    {
        public IMiddleware Next { get; set; }
        private readonly ICommandExecuter _commandExecuter;

        public CommandExecuterMiddleware()
        {
            _commandExecuter = new CommandExecuter();
        }

        public void Invoke<TResult>(ExecutionContext executionContext)
        {
            try
            {
                switch (executionContext.Type)
                {
                    case ExecutionType.Execute:
                        {
                            var recordsAffected = _commandExecuter.ExecuteNonQuery(executionContext);
                            executionContext.Result.SetData(recordsAffected);
                            return;
                        }
                    case ExecutionType.ExecuteScalar:
                        {
                            ParseExecuteScalarDbValue<TResult>(executionContext);
                            return;
                        }
                    case ExecutionType.GetDataSet:
                        {
                            var resultData = _commandExecuter.GetDateSet(executionContext);
                            executionContext.Result.SetData(resultData);
                            return;
                        }
                    case ExecutionType.GetDataTable:
                        {
                            var resultData = _commandExecuter.GetDateTable(executionContext);
                            executionContext.Result.SetData(resultData);
                            return;
                        }
                    case ExecutionType.Query:
                    case ExecutionType.QuerySingle:
                        {
                            executionContext.DataReaderWrapper = _commandExecuter.ExecuteReader(executionContext);
                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                Next.Invoke<TResult>(executionContext);
            }
            finally
            {
                if (executionContext.DataReaderWrapper != null)
                {
                    executionContext.DataReaderWrapper.Close();
                    executionContext.DataReaderWrapper.Dispose();
                }
            }
        }

        private void ParseExecuteScalarDbValue<TResult>(ExecutionContext executionContext)
        {
            var singleResult = executionContext.Result as SingleResultContext<TResult>;
            var dbResult = _commandExecuter.ExecuteScalar(executionContext);
            if (dbResult == DBNull.Value)
            {
                singleResult.SetData(default(TResult));
            }
            else
            {
                var convertType = singleResult.ResultType;
                convertType = Nullable.GetUnderlyingType(convertType) ?? convertType;
                
                if (convertType.IsEnum)
                {
                    singleResult.SetData(Enum.ToObject(convertType, dbResult));
                }
                else
                {
                    singleResult.SetData(Convert.ChangeType(dbResult, convertType));
                }
            }
        }


        private async Task ParseExecuteScalarDbValueAsync<TResult>(ExecutionContext executionContext)
        {
            var singleResult = executionContext.Result as SingleResultContext<TResult>;
            var dbResult = await _commandExecuter.ExecuteScalarAsync(executionContext);
            if (dbResult == null || dbResult == DBNull.Value)
            {
                singleResult.SetData(default(TResult));
            }
            else
            {
                var convertType = singleResult.ResultType;
                convertType = Nullable.GetUnderlyingType(convertType) ?? convertType;
                if (convertType.IsEnum)
                {
                    singleResult.SetData(Enum.ToObject(convertType, dbResult));
                }
                else
                {
                    singleResult.SetData(Convert.ChangeType(dbResult, convertType));
                }
            }
        }


        public async Task InvokeAsync<TResult>(ExecutionContext executionContext)
        {
            try
            {
                switch (executionContext.Type)
                {
                    case ExecutionType.Execute:
                        {
                            var recordsAffected = await _commandExecuter.ExecuteNonQueryAsync(executionContext);
                            executionContext.Result.SetData(recordsAffected);
                            return;
                        }
                    case ExecutionType.ExecuteScalar:
                        {
                            await ParseExecuteScalarDbValueAsync<TResult>(executionContext);
                            return;
                        }
                    case ExecutionType.GetDataSet:
                        {
                            var resultData = await _commandExecuter.GetDateSetAsync(executionContext);
                            executionContext.Result.SetData(resultData);
                            return;
                        }
                    case ExecutionType.GetDataTable:
                        {
                            var resultData = await _commandExecuter.GetDateTableAsync(executionContext);
                            executionContext.Result.SetData(resultData);
                            return;
                        }
                    case ExecutionType.Query:
                    case ExecutionType.QuerySingle:
                        {
                            executionContext.DataReaderWrapper = await _commandExecuter.ExecuteReaderAsync(executionContext);
                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                Next.Invoke<TResult>(executionContext);
            }
            finally
            {
                if (executionContext.DataReaderWrapper != null)
                {
                    executionContext.DataReaderWrapper.Close();
                    executionContext.DataReaderWrapper.Dispose();
                }
            }
        }
    }
}
