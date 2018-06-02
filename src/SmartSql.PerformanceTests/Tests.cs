using System;
using System.Diagnostics;

namespace SmartSql.PerformanceTests
{
    public class Tests
    {
        DapperBenchmarks _dapperBenchmarks;
        SmartSqlBenchmarks _smartSqlBenchmarks;
        SmartSqlDapperBenchmarks _smartSqlDapperBenchmarks;
        ChloeBenchmarks _chloeBenchmarks;
        public Tests()
        {
            _dapperBenchmarks = new DapperBenchmarks();
            _smartSqlBenchmarks = new SmartSqlBenchmarks();
            _smartSqlDapperBenchmarks = new SmartSqlDapperBenchmarks();
            _chloeBenchmarks = new ChloeBenchmarks();
            _smartSqlBenchmarks.Setup();
            _smartSqlDapperBenchmarks.Setup();
            _chloeBenchmarks.Setup();
        }
        int queryTimes = 100;
        public void QueryWrap()
        {
            try
            {
                _dapperBenchmarks.Query();
                _smartSqlBenchmarks.Query();
                _smartSqlDapperBenchmarks.Query();
                _chloeBenchmarks.Query();
                int smartSqlTimes = 0;
                int smartSqlDapperTimes = 0;
                int dapperTimes = 0;
                int chloeTimes = 0;
                Stopwatch stopwatch = Stopwatch.StartNew();
                while (dapperTimes < queryTimes)
                {
                    dapperTimes++;
                    _dapperBenchmarks.Query();
                }
                Console.WriteLine($"--------Dapper:QueryTimes:{queryTimes},QueryTaken:{BenchmarkBase.QUERY_TAKEN},TimeTaken:{stopwatch.ElapsedMilliseconds}ms--------");

                stopwatch.Restart();
                while (smartSqlTimes < queryTimes)
                {
                    smartSqlTimes++;
                    _smartSqlBenchmarks.Query();
                }
                Console.WriteLine($"--------SmartSql:QueryTimes:{queryTimes},QueryTaken:{BenchmarkBase.QUERY_TAKEN},TimeTaken:{stopwatch.ElapsedMilliseconds}ms--------");

                stopwatch.Restart();
                while (smartSqlDapperTimes < queryTimes)
                {
                    smartSqlDapperTimes++;
                    _smartSqlDapperBenchmarks.Query();
                }
                Console.WriteLine($"--------SmartSql_Dapper_Query:QueryTimes:{queryTimes},QueryTaken:{BenchmarkBase.QUERY_TAKEN},TimeTaken:{stopwatch.ElapsedMilliseconds}ms--------");

                stopwatch.Restart();
                while (chloeTimes < queryTimes)
                {
                    chloeTimes++;
                    _chloeBenchmarks.Query();
                }
                Console.WriteLine($"--------Chloe:QueryTimes:{queryTimes},QueryTaken:{BenchmarkBase.QUERY_TAKEN},TimeTaken:{stopwatch.ElapsedMilliseconds}ms--------");
                stopwatch.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("End");
        }

        public void InsertWarp()
        {
            _smartSqlBenchmarks.Insert();
            _dapperBenchmarks.Insert();
            _chloeBenchmarks.Insert();
        }
    }
}
