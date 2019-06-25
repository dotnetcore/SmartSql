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
        private readonly IConsumer<Null, string> _consumer;
        private readonly CancellationTokenSource _tokenSource;
        private Task _runTask;
        public event EventHandler<SyncRequest> Received;

        public KafkaSubscriber(KafkaOptions kafkaOptions, ILogger<KafkaSubscriber> logger)
        {
            _tokenSource = new CancellationTokenSource();
            _kafkaOptions = kafkaOptions;
            _logger = logger;
            _consumer = new ConsumerBuilder<Null, string>(kafkaOptions.AsKafkaConfig())
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
                var syncRequest = JsonConvert.DeserializeObject<SyncRequest>(result.Value);
                try
                {
                    Received?.Invoke(this, syncRequest);
                    if (_logger.IsEnabled(LogLevel.Debug))
                    {
                        _logger.LogDebug(
                            $"Received Invoke -> Id:[{syncRequest.Id}],Scope:[{syncRequest.Scope}],[{syncRequest.SqlId}] succeeded.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(new EventId(ex.HResult), ex,
                        $"Publish SyncRequest -> Id:{syncRequest.Id} failed, {nameof(KafkaOptions.Topic)}:[{_kafkaOptions.Topic}], {nameof(result.Offset)}:{result.Offset}.");
                }
            }
        }

        private void OnConsumeError(IConsumer<Null, string> consumer, Error error)
        {
            _logger.LogError($"{nameof(Error.Code)} :[{error.Code}] , {nameof(Error.Reason)}:[{error.Reason}]");
        }
    }
}