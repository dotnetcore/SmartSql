using SmartSql.Abstractions.DataReaderDeserializer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.Abstractions
{
    public interface IMultipleResult
    {
        IMultipleResult AddTypeMap<TResult>();
        IMultipleResult AddSingleTypeMap<TResult>();

        IMultipleResult InitData(RequestContext requestContext, IDataReaderWrapper dataReaderWrapper, IDataReaderDeserializer dataReaderDeserializer);
        Task<IMultipleResult> InitDataAsync(RequestContext requestContext, IDataReaderWrapper dataReaderWrapper, IDataReaderDeserializer dataReaderDeserializer);


        IEnumerable<TResult> Get<TResult>(int resultIndex);
        TResult GetSingle<TResult>(int resultIndex);
    }
}
