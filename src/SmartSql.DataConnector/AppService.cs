using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartSql.DataConnector.Configuration;
using Task = System.Threading.Tasks.Task;

namespace SmartSql.DataConnector
{
    public class AppService : BackgroundService
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly AppOptions _appOptions;
        private readonly IList<LoadTask> _loadTasks = new List<LoadTask>();

        public AppService(ILoggerFactory loggerFactory, IOptionsSnapshot<AppOptions> appOptions)
        {
            _loggerFactory = loggerFactory;
            _appOptions = appOptions.Value;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            foreach (var taskConf in _appOptions.Tasks)
            {
                var taskConfig = new TaskBuilder(taskConf.Path, _loggerFactory).Build();
                var loadTask = new LoadTask(taskConfig, _loggerFactory.CreateLogger<LoadTask>());
                _loadTasks.Add(loadTask);
                loadTask.Start();
            }

            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
            StopAll();
        }

        private void StopAll()
        {
            foreach (var loadTask in _loadTasks)
            {
                loadTask.Stop();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            StopAll();
        }
    }
}