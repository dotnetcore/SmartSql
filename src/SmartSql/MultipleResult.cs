using SmartSql.Abstractions;
using SmartSql.Abstractions.DataReaderDeserializer;
using SmartSql.Abstractions.DbSession;
using SmartSql.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql
{
    public class MultipleResult : IMultipleResult
    {
        private IDictionary<int, ResultMap> _resultDataMap = new Dictionary<int, ResultMap>();
        private int _resultIndex = 0;
        public MultipleResult() { }
        public MultipleResult(params Type[] resultTypes)
        {
            foreach (var type in resultTypes)
            {
                bool isEnum = typeof(IEnumerable).IsAssignableFrom(type);
                var resultType = isEnum ? type.GenericTypeArguments[0] : type;
                _resultDataMap.Add(_resultIndex, new ResultMap
                {
                    Index = _resultIndex,
                    Type = isEnum ? ResultMap.ResultTypeType.Enumerable : ResultMap.ResultTypeType.Single,
                    ResultType = resultType
                });
                _resultIndex++;
            }
        }
        public IMultipleResult InitData(RequestContext requestContext, IDataReaderWrapper dataReaderWrapper, IDataReaderDeserializer dataReaderDeserializer)
        {
            foreach (var resultMapKV in _resultDataMap)
            {
                var resultMap = resultMapKV.Value;

                switch (resultMap.Type)
                {
                    case ResultMap.ResultTypeType.Single:
                        {
                            resultMap.Result = dataReaderDeserializer.ToSingle(requestContext, dataReaderWrapper, resultMap.ResultType, false);
                            break;
                        }
                    case ResultMap.ResultTypeType.Enumerable:
                        {
                            resultMap.Result = dataReaderDeserializer.ToEnumerable(requestContext, dataReaderWrapper, resultMap.ResultType, false);
                            break;
                        }
                }
                dataReaderWrapper.NextResult();
            }
            return this;
        }
        public async Task<IMultipleResult> InitDataAsync(RequestContext requestContext, IDataReaderWrapper dataReaderWrapper, IDataReaderDeserializer dataReaderDeserializer)
        {
            foreach (var resultMapKV in _resultDataMap)
            {
                var resultMap = resultMapKV.Value;

                switch (resultMap.Type)
                {
                    case ResultMap.ResultTypeType.Single:
                        {
                            resultMap.Result = await dataReaderDeserializer.ToSingleAsync(requestContext, dataReaderWrapper, resultMap.ResultType, false);
                            break;
                        }
                    case ResultMap.ResultTypeType.Enumerable:
                        {
                            resultMap.Result = await dataReaderDeserializer.ToEnumerableAsync(requestContext, dataReaderWrapper, resultMap.ResultType, false);
                            break;
                        }
                }
                await dataReaderWrapper.NextResultAsync();
            }
            return this;
        }

        public IMultipleResult AddSingleTypeMap<TResult>()
        {
            _resultDataMap.Add(_resultIndex, new ResultMap
            {
                Index = _resultIndex,
                Type = ResultMap.ResultTypeType.Single,
                ResultType = typeof(TResult)
            });
            _resultIndex++;
            return this;
        }

        public IMultipleResult AddTypeMap<TResult>()
        {
            _resultDataMap.Add(_resultIndex, new ResultMap
            {
                Index = _resultIndex,
                Type = ResultMap.ResultTypeType.Enumerable,
                ResultType = typeof(TResult)
            });
            _resultIndex++;
            return this;
        }

        public IEnumerable<TResult> Get<TResult>(int resultIndex)
        {
            var result = (IEnumerable<object>)_resultDataMap[resultIndex].Result;
            return result.Select(m => (TResult)m);
        }

        public TResult GetSingle<TResult>(int resultIndex)
        {
            return (TResult)_resultDataMap[resultIndex].Result;
        }

        public class ResultMap
        {
            public int Index { get; set; }
            public ResultTypeType Type { get; set; }
            public Type ResultType { get; set; }
            public object Result { get; set; }

            public enum ResultTypeType
            {
                Single = 1,
                Enumerable = 2
            }
        }
    }

}
