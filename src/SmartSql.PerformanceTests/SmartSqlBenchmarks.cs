using BenchmarkDotNet.Attributes;
using SmartSql.Abstractions;
using SmartSql.DataReaderDeserializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartSql.PerformanceTests
{
    public class SmartSqlBenchmarks : BenchmarkBase
    {
        ISmartSqlMapper _sqlMapper;
        DataRowParserFactory _dataRowParserFactory;

        [GlobalSetup]
        public void Setup()
        {
            _dataRowParserFactory = new DataRowParserFactory();
            var smartSqlOptions = new SmartSqlOptions
            {
            };
            _sqlMapper = new SmartSqlMapper(smartSqlOptions);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _sqlMapper.Dispose();
        }
        //[Benchmark]
        public override long Insert()
        {
            return _sqlMapper.ExecuteScalar<long>(new RequestContext
            {
                Scope = "Entity",
                SqlId = "Insert",
                Request = GetInsertEntity()
            });
        }
        [Benchmark]
        public override IEnumerable<T_Entity> Query()
        {
            var list = _sqlMapper.Query<T_Entity>(new RequestContext
            {
                Scope = "Entity",
                SqlId = "Query",
                Request = new { Taken = QUERY_TAKEN }
            });
            return list;
        }

        public override int Update()
        {
            throw new NotImplementedException();
        }
        //[Benchmark]
        public IEnumerable<T_Entity> Parser()
        {
            var list = new List<T_Entity>();
            WrapParser((dataReader) =>
            {
                if (dataReader.Read())
                {
                    var reqCtx = new RequestContext
                    {
                        Scope = "T_Entity",
                        SqlId = "Query"
                    };
                    var wrapper = new DataReaderWrapper(dataReader);
                    var deser = _dataRowParserFactory.GetParser(wrapper, reqCtx, typeof(T_Entity));
                    do
                    {
                        var obj = (T_Entity)deser.Invoke(dataReader, reqCtx);
                        list.Add(obj);
                    } while (dataReader.Read());
                }
                dataReader.Dispose();
            });
            return list;
        }

    }
}
