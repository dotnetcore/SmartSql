using System;
using System.Threading.Tasks;

namespace SmartSql.InvokeSync.Kafka
{
    public class KafkaSubscriber:ISubscriber
    {
        public event EventHandler<SyncRequest> Received;

        public void Listening()
        {
            throw new System.NotImplementedException();
        }
    }
}