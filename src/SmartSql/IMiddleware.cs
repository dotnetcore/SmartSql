using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql
{
    public interface IMiddleware : IOrdered
    {
        IMiddleware Next { get; set; }
        void Invoke<TResult>(ExecutionContext executionContext);
        Task InvokeAsync<TResult>(ExecutionContext executionContext);
    }
}