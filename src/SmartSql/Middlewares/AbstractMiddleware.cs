using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartSql.Middlewares.Filters;

namespace SmartSql.Middlewares
{
    public abstract class AbstractMiddleware : IMiddleware, ISetupSmartSql
    {
        protected virtual Type FilterType { get; }
        public IMiddleware Next { get; set; }
        protected IList<IInvokeMiddlewareFilter> Filters { get; set; }

        public virtual void Invoke<TResult>(ExecutionContext executionContext)
        {
            OnInvoking(executionContext);
            SelfInvoke<TResult>(executionContext);
            OnInvoked(executionContext);
            if (!executionContext.Result.End)
            {
                InvokeNext<TResult>(executionContext);
                OnNextInvoked<TResult>(executionContext);
            }
        }

        protected virtual void OnNextInvoked<TResult>(ExecutionContext executionContext)
        {
        }

        protected virtual Task OnNextInvokedAsync<TResult>(ExecutionContext executionContext)
        {
            return Task.CompletedTask;
        }

        protected virtual void SelfInvoke<TResult>(ExecutionContext executionContext)
        {
        }

        protected virtual Task SelfInvokeAsync<TResult>(ExecutionContext executionContext)
        {
            return Task.CompletedTask;
        }

        protected void InvokeNext<TResult>(ExecutionContext executionContext)
        {
            Next?.Invoke<TResult>(executionContext);
        }

        public virtual async Task InvokeAsync<TResult>(ExecutionContext executionContext)
        {
            await OnInvokingAsync(executionContext);
            await SelfInvokeAsync<TResult>(executionContext);
            await OnInvokedAsync(executionContext);
            if (!executionContext.Result.End)
            {
                await InvokeNextAsync<TResult>(executionContext);
                await OnNextInvokedAsync<TResult>(executionContext);
            }
        }

        protected async Task InvokeNextAsync<TResult>(ExecutionContext executionContext)
        {
            if (Next != null)
            {
                await Next.InvokeAsync<TResult>(executionContext);
            }
        }

        #region Filter

        protected void OnInvoking(ExecutionContext executionContext)
        {
            if (Filters == null)
            {
                return;
            }

            foreach (var filter in Filters)
            {
                filter.OnInvoking(executionContext);
            }
        }

        protected void OnInvoked(ExecutionContext executionContext)
        {
            if (Filters == null)
            {
                return;
            }

            foreach (var filter in Filters)
            {
                filter.OnInvoked(executionContext);
            }
        }

        protected async Task OnInvokingAsync(ExecutionContext executionContext)
        {
            if (Filters == null)
            {
                return;
            }

            foreach (var filter in Filters)
            {
                await filter.OnInvokingAsync(executionContext);
            }
        }

        protected async Task OnInvokedAsync(ExecutionContext executionContext)
        {
            if (Filters == null)
            {
                return;
            }

            foreach (var filter in Filters)
            {
                await filter.OnInvokedAsync(executionContext);
            }
        }

        #endregion

        protected void InitFilters(SmartSqlBuilder smartSqlBuilder)
        {
            if (FilterType == null)
            {
                return;
            }

            Filters = smartSqlBuilder.SmartSqlConfig.Filters.Where(f => FilterType.IsInstanceOfType(f))
                .Select(f => f as IInvokeMiddlewareFilter).ToList();
        }

        public virtual void SetupSmartSql(SmartSqlBuilder smartSqlBuilder)
        {
            InitFilters(smartSqlBuilder);
        }

        public abstract int Order { get; }
    }
}