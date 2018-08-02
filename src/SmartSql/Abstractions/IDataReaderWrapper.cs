using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.Abstractions
{
    public interface IDataReaderWrapper : IDataReader
    {
        int ResultIndex { get; }
        Task<bool> ReadAsync();
        Task<bool> NextResultAsync();

        //IEnumerable<T> Read<T>();
        //T ReadSingle<T>();

        //Task<IEnumerable<T>> ReadAsync<T>();
        //Task<T> ReadSingleAsync<T>();
    }
}

