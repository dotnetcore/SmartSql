using SmartSql.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.Deserializer
{
    public interface IDataReaderDeserializer : IDataReaderDeserializerAsync
    {
        bool CanDeserialize(ExecutionContext executionContext, Type resultType, bool isMultiple = false);
        TResult ToSingle<TResult>(ExecutionContext executionContext);
        IList<TResult> ToList<TResult>(ExecutionContext executionContext);
    }
}
