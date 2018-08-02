using SmartSql.Abstractions;
using SmartSql.Abstractions.DataReaderDeserializer;
using SmartSql.Abstractions.DbSession;
using SmartSql.Exceptions;
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
        private IDataReaderWrapper _dataReaderWrapper;
        private readonly IDataReaderDeserializer _dataReaderDeserializer;
        private readonly IDbConnectionSessionStore _sessionStore;

        public MultipleResult(
            RequestContext context
            , IDataReaderWrapper dataReaderWrapper
            , IDataReaderDeserializer dataReaderDeserializer
            , IDbConnectionSessionStore sessionStore
            )
        {
            _context = context;
            _dataReaderWrapper = dataReaderWrapper;
            _dataReaderDeserializer = dataReaderDeserializer;
            _sessionStore = sessionStore;
        }
        public T ReadSingle<T>()
        {
            CheckDbReader();
            var result = _dataReaderDeserializer.ToSingle<T>(_context, _dataReaderWrapper, false);
            NextResult();
            return result;
        }

        private void CheckDbReader()
        {
            if (_dataReaderWrapper == null)
            {
                throw new SmartSqlException("no more result!");
            }
        }

        public IEnumerable<T> Read<T>()
        {
            CheckDbReader();
            var result = _dataReaderDeserializer.ToEnumerable<T>(_context, _dataReaderWrapper, false).ToList();
            NextResult();
            return result;
        }

        private void NextResult()
        {
            if (!_dataReaderWrapper.NextResult())
            {
                Dispose();
            }
        }
        private async Task NextResultAsync()
        {
            if (!await _dataReaderWrapper.NextResultAsync())
            {
                Dispose();
            }
        }
        public void Dispose()
        {
            if (_dataReaderWrapper != null)
            {
                _dataReaderWrapper.Dispose();
                _dataReaderWrapper = null;
            }
            _sessionStore.Dispose();
        }

        public async Task<T> ReadSingleAsync<T>()
        {
            CheckDbReader();
            var result = await _dataReaderDeserializer.ToSingleAsync<T>(_context, _dataReaderWrapper, false);
            await NextResultAsync();
            return result;
        }

        public async Task<IEnumerable<T>> ReadAsync<T>()
        {
            CheckDbReader();
            var result = await _dataReaderDeserializer.ToEnumerableAsync<T>(_context, _dataReaderWrapper, false);
            await NextResultAsync();
            return result;
        }
    }
}
