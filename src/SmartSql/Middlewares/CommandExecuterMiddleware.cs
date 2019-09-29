using SmartSql.Command;
using SmartSql.Configuration;
using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SmartSql.Middlewares
{
    public class CommandExecuterMiddleware : AbstractMiddleware
    {
        private ICommandExecuter _commandExecuter;

        public override void Invoke<TResult>(ExecutionContext executionContext)
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
                    InvokeNext<TResult>(executionContext);
                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ParseExecuteScalarDbValue<TResult>(ExecutionContext executionContext)
        {
            var singleResult = executionContext.Result as SingleResultContext<TResult>;
            var dbResult = _commandExecuter.ExecuteScalar(executionContext);
            SetResultData(dbResult, singleResult);
        }

        private void SetResultData<TResult>(object dbResult, SingleResultContext<TResult> singleResult)
        {
            if (dbResult == null || dbResult == DBNull.Value)
            {
                singleResult.SetData(default(TResult));
            }
            else
            {
                var convertType = singleResult.ResultType;
                convertType = Nullable.GetUnderlyingType(convertType) ?? convertType;

                if (convertType.IsInstanceOfType(dbResult))
                {
                    singleResult.SetData(dbResult);
                    return;
                }

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
            SetResultData(dbResult, singleResult);
        }


        public override async Task InvokeAsync<TResult>(ExecutionContext executionContext)
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
                    await InvokeNextAsync<TResult>(executionContext);
                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void SetupSmartSql(SmartSqlBuilder smartSqlBuilder)
        {
            _commandExecuter = smartSqlBuilder.SmartSqlConfig.CommandExecuter;
        }

        public override int Order => 500;
    }
}