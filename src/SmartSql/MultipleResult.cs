using SmartSql.Abstractions;
using SmartSql.Abstractions.DataReaderDeserializer;
using SmartSql.Abstractions.DbSession;
using System;
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
        private readonly RequestContext _context;
        private IDataReader _dataReader;
        private readonly IDataReaderDeserializer _dataReaderDeserializer;
        private readonly IDbConnectionSessionStore _sessionStore;

        public MultipleResult(
            RequestContext context
            , IDataReader dataReader
            , IDataReaderDeserializer dataReaderDeserializer
            , IDbConnectionSessionStore sessionStore
            )
        {
            _context = context;
            _dataReader = dataReader;
            _dataReaderDeserializer = dataReaderDeserializer;
            _sessionStore = sessionStore;
        }
        public T ReadSingle<T>()
        {
            var result = _dataReaderDeserializer.ToSingle<T>(_context, _dataReader, false);
            NextResult();
            return result;
        }
        public IEnumerable<T> Read<T>()
        {
            var result = _dataReaderDeserializer.ToEnumerable<T>(_context, _dataReader, false).ToList();
            NextResult();
            return result;
        }

        private void NextResult()
        {
            if (!_dataReader.NextResult())
            {
                Dispose();
            }
        }
        private async Task NextResultAsync()
        {
            var dataReaderAsync = _dataReader as DbDataReader;
            if (!await dataReaderAsync.NextResultAsync())
            {
                Dispose();
            }
        }
        public void Dispose()
        {
            if (_dataReader != null)
            {
                _dataReader.Dispose();
                _dataReader = null;
            }
            _sessionStore.Dispose();
        }

        public async Task<T> ReadSingleAsync<T>()
        {
            var result = await _dataReaderDeserializer.ToSingleAsync<T>(_context, _dataReader, false);
            await NextResultAsync();
            return result;
        }

        public async Task<IEnumerable<T>> ReadAsync<T>()
        {
            var result = await _dataReaderDeserializer.ToEnumerableAsync<T>(_context, _dataReader, false);
            await NextResultAsync();
            return result;
        }
    }
}
