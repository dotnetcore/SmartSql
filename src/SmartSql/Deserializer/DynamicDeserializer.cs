using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SmartSql.Data;
using SmartSql.Reflection.TypeConstants;

namespace SmartSql.Deserializer
{
    public class DynamicDeserializer : IDataReaderDeserializer
    {
        public bool CanDeserialize(ExecutionContext executionContext, Type resultType, bool isMultiple = false)
        {
            return resultType == CommonType.Object || resultType == CommonType.DictionaryStringObject ||
                   resultType == DataType.DynamicRow;
        }

        public TResult ToSingle<TResult>(ExecutionContext executionContext)
        {
            var dataReader = executionContext.DataReaderWrapper;
            if (!dataReader.HasRows) return default;
            dataReader.Read();
            var columns = GetColumns(dataReader);
            object dyRow = ToDynamicRow(dataReader, columns);
            return (TResult) dyRow;
        }

        public IList<TResult> ToList<TResult>(ExecutionContext executionContext)
        {
            var list = new List<TResult>();
            var dataReader = executionContext.DataReaderWrapper;
            if (!dataReader.HasRows) return list;
            var columns = GetColumns(dataReader);
            while (dataReader.Read())
            {
                object dyRow = ToDynamicRow(dataReader, columns);
                list.Add((TResult) dyRow);
            }

            return list;
        }

        public async Task<TResult> ToSingleAsync<TResult>(ExecutionContext executionContext)
        {
            var dataReader = executionContext.DataReaderWrapper;
            if (!dataReader.HasRows) return default;
            await dataReader.ReadAsync();
            var columns = GetColumns(dataReader);
            object dyRow = ToDynamicRow(dataReader, columns);
            return (TResult) dyRow;
        }

        public async Task<IList<TResult>> ToListAsync<TResult>(ExecutionContext executionContext)
        {
            var list = new List<TResult>();
            var dataReader = executionContext.DataReaderWrapper;
            if (!dataReader.HasRows) return list;
            var columns = GetColumns(dataReader);
            while (await dataReader.ReadAsync())
            {
                object dyRow = ToDynamicRow(dataReader, columns);
                list.Add((TResult) dyRow);
            }

            return list;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DynamicRow ToDynamicRow(DataReaderWrapper dataReader, IDictionary<string, int> columns)
        {
            var values = new object[columns.Count];
            dataReader.GetValues(values);
            return new DynamicRow(columns, values);
        }

        private IDictionary<string, int> GetColumns(DataReaderWrapper dataReader)
        {
            return Enumerable.Range(0, dataReader.FieldCount)
                .Select(i => new KeyValuePair<string, int>(dataReader.GetName(i), i))
                .ToDictionary(kv => kv.Key, kv => kv.Value);
        }
    }
}