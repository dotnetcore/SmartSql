using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SmartSql.Test.Unit.ErrorDiagnosis
{
    [Collection("GlobalSmartSql")]
    public class ErrorDiagnosisTest
    {
        private readonly SmartSqlFixture _smartSqlFixture;
        private readonly ITestOutputHelper _output;
        protected ISqlMapper SqlMapper { get; }

        public ErrorDiagnosisTest(SmartSqlFixture smartSqlFixture, ITestOutputHelper output)
        {
            _smartSqlFixture = smartSqlFixture;
            _output = output;
            SqlMapper = smartSqlFixture.SqlMapper;
        }
        [Fact]
        public void Error()
        {
            try
            {
                SqlMapper.Execute(new RequestContext
                {
                    RealSql = "Error"
                });
            }
            catch (Exception e)
            {
                _output.WriteLine(e.StackTrace);
                Assert.True(true);
            }
        }
        [Fact]
        public async Task ErrorAsync()
        {
            try
            {
                await SqlMapper.ExecuteAsync(new RequestContext
                {
                    RealSql = "Error"
                });
            }
            catch (Exception e)
            {
                _output.WriteLine(e.StackTrace);
                Assert.True(true);
            }
        }
        [Fact]
        public void OpenError()
        {
            try
            {
                var dbSession = _smartSqlFixture.DbSessionFactory.Open("");
                dbSession.Open();
            }
            catch (Exception e)
            {
                _output.WriteLine(e.StackTrace);
                Assert.True(true);
            }
        }
    }
}
