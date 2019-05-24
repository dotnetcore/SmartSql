using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace SmartSql.DbSession
{
    public class InvokeSucceedEventArgs : EventArgs
    {
        public ExecutionContext ExecutionContext { get; set; }
    }

    public delegate void InvokeSucceedEventHandler(object sender, InvokeSucceedEventArgs eventArgs);

    public class InvokeSucceedListener
    {
        private readonly ConcurrentDictionary<Guid, ConcurrentQueue<ExecutionContext>> _sessionMappedExecutionQueue;
        public event InvokeSucceedEventHandler InvokeSucceed;

        public InvokeSucceedListener()
        {
            _sessionMappedExecutionQueue = new ConcurrentDictionary<Guid, ConcurrentQueue<ExecutionContext>>();
        }

        public void BindDbSessionEvent(IDbSession dbSession)
        {
            dbSession.Rollbacked += (sender, args) => { OnDbSessionRollbacked(sender as IDbSession); };
            dbSession.Disposed += (sender, args) => { OnDbSessionDisposed(sender as IDbSession); };
            dbSession.Committed += (sender, args) => { OnDbSessionCommitted(sender as IDbSession); };
            dbSession.Invoked += (sender, args) => { OnDbSessionInvoked(args.ExecutionContext); };
        }

        private void OnDbSessionInvoked(ExecutionContext executionContext)
        {
            if (executionContext.DbSession.Transaction == null)
            {
                InvokeSucceed?.Invoke(this, new InvokeSucceedEventArgs {ExecutionContext = executionContext});
            }
            else
            {
                if (!_sessionMappedExecutionQueue.TryGetValue(executionContext.DbSession.Id, out var executionQueue))
                {
                    executionQueue = new ConcurrentQueue<ExecutionContext>();
                    _sessionMappedExecutionQueue.TryAdd(executionContext.DbSession.Id, executionQueue);
                }

                executionQueue.Enqueue(executionContext);
            }
        }

        private void OnDbSessionRollbacked(IDbSession dbSession)
        {
            _sessionMappedExecutionQueue.TryRemove(dbSession.Id, out _);
        }

        private void OnDbSessionDisposed(IDbSession dbSession)
        {
            if (_sessionMappedExecutionQueue.TryGetValue(dbSession.Id, out _))
            {
                _sessionMappedExecutionQueue.TryRemove(dbSession.Id, out _);
            }
        }

        private void OnDbSessionCommitted(IDbSession dbSession)
        {
            if (!_sessionMappedExecutionQueue.TryGetValue(dbSession.Id, out var executionQueue)) return;
            while (executionQueue.TryDequeue(out var executionContext))
            {
                InvokeSucceed?.Invoke(this, new InvokeSucceedEventArgs {ExecutionContext = executionContext});
            }

            _sessionMappedExecutionQueue.TryRemove(dbSession.Id, out _);
        }
    }
}