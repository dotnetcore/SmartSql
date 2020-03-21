using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SmartSql.Configuration;
using SmartSql.DataConnector.Configuration;
using SmartSql.InvokeSync;
using SmartSql.InvokeSync.Utils;
using SmartSql.Utils;

namespace SmartSql.DataConnector
{
    public class LoadTask : ITask
    {
        private readonly ILogger<LoadTask> _logger;
        private static readonly InsertWithId _insertWithId = new InsertWithId();
        private static readonly TableNameAnalyzer _tableNameAnalyzer = new TableNameAnalyzer();
        private static StatementAnalyzer _statementAnalyzer = new StatementAnalyzer();
        public Task Task { get; }

        public LoadTask(Task task, ILogger<LoadTask> logger)
        {
            _logger = logger;
            Task = task;
        }

        public void Start()
        {
            Task.Subscriber.Instance.Received += OnReceived;
            Task.Subscriber.Instance.Start();
        }

        public void Stop()
        {
            Task.Subscriber.Instance.Stop();
        }

        private void OnReceived(object sender, SyncRequest syncRequest)
        {
            if (!syncRequest.StatementType.HasValue)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug(
                        $"SyncRequest.Id:[{syncRequest.Id}] StatementType has non-value.");
                }

                return;
            }

            var destParameterPrefix =
                Task.DataSource.Instance.SmartSqlConfig.Database.DbProvider.ParameterPrefix;

            foreach (var job in Task.Jobs)
            {
                try
                {
                    var source = job.Value.Source;
                    if (!String.Equals(source.Scope, syncRequest.Scope, StringComparison.OrdinalIgnoreCase))
                    {
                        if (_logger.IsEnabled(LogLevel.Debug))
                        {
                            _logger.LogDebug(
                                $"Job:[{job.Key}],Scope:[{source.Scope}] not equals SyncRequest.Scope:[{syncRequest.Scope}].");
                        }

                        continue;
                    }

                    if (source.SqlIds != null
                        && source.SqlIds.Length > 0
                        && !source.SqlIds.Contains(syncRequest.SqlId, StringComparer.OrdinalIgnoreCase))
                    {
                        if (_logger.IsEnabled(LogLevel.Debug))
                        {
                            _logger.LogDebug(
                                $"Job:[{job.Key}],SqlIds non-contains SyncRequest.SqlId:[{syncRequest.SqlId}].");
                        }

                        continue;
                    }

                    var dest = job.Value.Dest;
                    var sql = syncRequest.RealSql;
                    var sourceParameterPrefix = syncRequest.ParameterPrefix;
                    var statementType = syncRequest.StatementType.Value;

                    if (statementType.HasFlag(StatementType.Insert)
                        && source.PrimaryKey?.IsAutoIncrement == true)
                    {
                        sql = _insertWithId.Replace(sql, source.PrimaryKey.Name, dest.PrimaryKey.Name,
                            destParameterPrefix);
                        syncRequest.Parameters.Add(dest.PrimaryKey.Name, syncRequest.Result);
                        if (statementType.HasFlag(StatementType.Select))
                        {
                            sql = sql.Split(';')[0];
                        }
                    }
                    
                    if (!String.IsNullOrEmpty(sourceParameterPrefix)
                        && sourceParameterPrefix != destParameterPrefix)
                    {
                        sql = sql.Replace(sourceParameterPrefix, destParameterPrefix);
                    }

                    sql = _tableNameAnalyzer.Replace(statementType, sql, (tableName, op) => op + dest.TableName);

                    var affected = Task.DataSource.Instance.SqlMapper.Execute(new RequestContext
                    {
                        RealSql = sql,
                        Request = syncRequest.Parameters
                    });
                    if (affected < 1)
                    {
                        _logger.LogError(
                            $"Job:[{job.Key}],Execute failed! Affected:[{affected}]. SyncRequest: {Environment.NewLine}[{JsonConvert.SerializeObject(syncRequest)}].");
                    }
                    else
                    {
                        if (_logger.IsEnabled(LogLevel.Information))
                        {
                            _logger.LogInformation($"Job:[{job.Key}],Execute succeed! SyncRequest.Id:[{syncRequest.Id}].");
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e,
                        $"Job:[{job.Key}],Execute failed! SyncRequest: {Environment.NewLine}[{JsonConvert.SerializeObject(syncRequest)}]."
                    );
                }
            }
        }
    }
}