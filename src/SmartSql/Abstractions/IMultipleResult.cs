using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.Abstractions
{
    public interface IMultipleResult : IDisposable
    {
        T ReadSingle<T>();
        IEnumerable<T> Read<T>();

        Task<T> ReadSingleAsync<T>();
        Task<IEnumerable<T>> ReadAsync<T>();
    }
}
