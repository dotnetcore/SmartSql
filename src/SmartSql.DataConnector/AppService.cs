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
        private readonly ILogger<AppService> _logger;

        public AppService(ILoggerFactory loggerFactory, IOptionsSnapshot<AppOptions> appOptions)
        {
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<AppService>();
            _appOptions = appOptions.Value;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ExecuteAsync.");
            foreach (var task in _appOptions.Tasks)
            {
                _logger.LogInformation($"Task.Path:[{task.Path}] Loading.");
                var taskConfig = new TaskBuilder(task.Path, _loggerFactory).Build();
                var loadTask = new LoadTask(taskConfig, _loggerFactory.CreateLogger<LoadTask>());
                _loadTasks.Add(loadTask);
                loadTask.Start();
                _logger.LogInformation($"Task.Path:[{task.Path}] Start.");
            }

            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StopAsync.");
            StopAll();
            await base.StopAsync(cancellationToken);
            
        }

        private void StopAll()
        {
            foreach (var loadTask in _loadTasks)
            {
                loadTask.Stop();
                _logger.LogInformation($"LoadTask :[{loadTask.Task.Name}] Stop.");
            }
        }

        public override void Dispose()
        {
            _logger.LogInformation("Dispose.");
            base.Dispose();
        }
    }
}