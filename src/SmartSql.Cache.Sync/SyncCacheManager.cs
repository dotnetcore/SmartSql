using System;
using SmartSql.Configuration;
using SmartSql.InvokeSync;

namespace SmartSql.Cache.Sync
{
    public class SyncCacheManager : AbstractCacheManager
    {
        private readonly ISubscriber _subscriber;

        public SyncCacheManager(ISubscriber subscriber)
        {
            _subscriber = subscriber;
        }

        protected override void ListenInvokeSucceeded()
        {
            _subscriber.Received += SubscriberOnReceived;
        }

        private void SubscriberOnReceived(object sender, SyncRequest e)
        {
            if (!e.IsStatementSql)
            {
                return;
            }

            FlushOnExecuted($"{e.Scope}.{e.SqlId}");
        }
    }
}