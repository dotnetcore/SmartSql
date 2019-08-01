using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SmartSql.InvokeSync.Kafka
{
    public class KafkaSubscriber : ISubscriber
    {
        private readonly KafkaOptions _kafkaOptions;
        private readonly ILogger<KafkaSubscriber> _logger;
        private readonly IConsumer<String, string> _consumer;
        private readonly CancellationTokenSource _tokenSource;
        private Task _runTask;
        public event EventHandler<SyncRequest> Received;

        public KafkaSubscriber(KafkaOptions kafkaOptions, ILogger<KafkaSubscriber> logger)
        {
            _tokenSource = new CancellationTokenSource();
            _kafkaOptions = kafkaOptions;
            _logger = logger;
            _consumer = new ConsumerBuilder<String, string>(kafkaOptions.AsKafkaConfig())
                .SetErrorHandler(OnConsumeError)
                .Build();
        }

        public void Start()
        {
            _consumer.Subscribe(_kafkaOptions.Topic);
            _runTask = Task.Factory.StartNew(Loop, TaskCreationOptions.LongRunning);
        }

        public void Stop()
        {
            _tokenSource.Cancel();
            _runTask.Dispose();
            _consumer.Dispose();
        }

        private void Loop()
        {
            while (!_tokenSource.IsCancellationRequested)
            {
                var result = _consumer.Consume();
                var syncMsg = JsonConvert.DeserializeObject<SyncRequest>(result.Value);
                try
                {
                    Received?.Invoke(this, syncMsg);
                    if (_logger.IsEnabled(LogLevel.Debug))
                    {
                        _logger.LogDebug(
                            $"Received Invoke -> Id:[{syncMsg.Id}],Scope:[{syncMsg.Scope}],[{syncMsg.SqlId}] succeeded.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(new EventId(ex.HResult), ex,
                        $"Received Invoke -> Id:{syncMsg.Id} failed, {nameof(KafkaOptions.Topic)}:[{_kafkaOptions.Topic}], {nameof(result.Offset)}:{result.Offset}. {Environment.NewLine} -> SyncRequest: [{result.Value}]");
                }
            }
        }

        private void OnConsumeError(IConsumer<String, string> consumer, Error error)
        {
            _logger.LogError($"{nameof(Error.Code)} :[{error.Code}] , {nameof(Error.Reason)}:[{error.Reason}]");
        }
    }
}