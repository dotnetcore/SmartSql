using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSql.Deserializer
{
    public interface IDataReaderDeserializerAsync
    {
        Task<TResult> ToSingeAsync<TResult>(ExecutionContext executionContext);
        Task<IList<TResult>> ToListAsync<TResult>(ExecutionContext executionContext);
    }
}
