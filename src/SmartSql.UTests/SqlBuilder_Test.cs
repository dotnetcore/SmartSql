using SmartSql.Abstractions;
using Microsoft.Extensions.Logging;
using SmartSql.Abstractions.Config;
using Xunit;

namespace SmartSql.UTests
{
    public class SqlBuilder_Test : TestBase
    {
        ISqlBuilder _sqlBuilder;
        IConfigLoader _configLoader;
        public SqlBuilder_Test()
        {
            _configLoader = new LocalFileConfigLoader(SqlMapConfigFilePath, LoggerFactory);
            var smartSqlContext = new SmartSqlContext(LoggerFactory.CreateLogger<SmartSqlContext>(), _configLoader.Load());
            _sqlBuilder = new SqlBuilder(LoggerFactory.CreateLogger<SqlBuilder>(), smartSqlContext);

        }
        [Fact]
        public void BuildSql()
        {

            RequestContext context = new RequestContext
            {
                Scope = "T_Entity",
                SqlId = "Query",
                Request = new
                {
                    FString = "SmartSql",
                    NullStatus = 1,
                    Ids = new long[] { 1, 23, 4, 5, 6 }
                }
            };
            string rightSql = "SELECT T.* From T_Entity T With(NoLock) Order By  T.FLong Desc";
            string sql = _sqlBuilder.BuildSql(context);
            Assert.Equal<string>(rightSql, sql);
        }

    }
}
