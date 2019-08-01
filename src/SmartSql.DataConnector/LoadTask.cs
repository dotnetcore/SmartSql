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
            foreach (var job in Task.Jobs)
            {
                var source = job.Value.Source;
                if (!String.Equals(source.Scope, syncRequest.Scope, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (source.SqlIds != null
                    && source.SqlIds.Length > 0
                    && !source.SqlIds.Contains(syncRequest.SqlId, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                var dest = job.Value.Dest;
                var sql = syncRequest.RealSql;
                var sourceParameterPrefix = syncRequest.ParameterPrefix;
                var destParameterPrefix = Task.DataSource.Instance.SmartSqlConfig.Database.DbProvider.ParameterPrefix;
                if (syncRequest.StatementType == StatementType.Insert
                    && source.PrimaryKey?.IsAutoIncrement == true)
                {
                    sql = _insertWithId.Replace(sql, source.PrimaryKey.Name, dest.PrimaryKey.Name, destParameterPrefix);
                    syncRequest.Parameters.Add(dest.PrimaryKey.Name, syncRequest.Result);
                }

                if (sourceParameterPrefix != destParameterPrefix)
                {
                    sql = sql.Replace(sourceParameterPrefix, destParameterPrefix);
                }

                sql = _tableNameAnalyzer.Replace(syncRequest.StatementType.Value, sql,
                    (tableName, op) => op + dest.TableName);

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
            }
        }
    }
}