using SmartSql.Abstractions;
using Microsoft.Extensions.Logging;
using SmartSql.Abstractions.Config;
using Xunit;

namespace SmartSql.UTests
{
    public class SqlBuilder_Test : TestBase
    {
        public override string Scope => "SqlBuilder";
        ISqlBuilder _sqlBuilder;
        IConfigLoader _configLoader;
        SmartSqlContext _smartSqlContext;
        public SqlBuilder_Test()
        {
            _configLoader = new LocalFileConfigLoader(SqlMapConfigFilePath, LoggerFactory);
            _smartSqlContext = new SmartSqlContext(LoggerFactory.CreateLogger<SmartSqlContext>(), _configLoader.Load());
            _sqlBuilder = new SqlBuilder(LoggerFactory.CreateLogger<SqlBuilder>(), _smartSqlContext, _configLoader);
        }
        [Fact]
        public void Placeholder_Test()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "Placeholder_Test",
                Request = new
                {
                    OrderBy = "Id Desc"
                }
            };
            context.Setup(_smartSqlContext);
            _sqlBuilder.BuildSql(context);
            string sql = "Select T.* From T_Entity T Order By Id Desc";
             Assert.Equal<string>(context.RealSql, sql);
        }

        [Fact]
        public void Placeholder_Inclue_Test()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "Placeholder_Include_Test",
                Request = new
                {
                    OrderBy = "Id Desc"
                }
            };
            context.Setup(_smartSqlContext);
            _sqlBuilder.BuildSql(context);
            string sql = "Select T.* From T_Entity T Order By Id Desc";
            Assert.Equal<string>(context.RealSql, sql);
        }

        [Fact]
        public void Placeholder_Table_Test()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "Placeholder_Table_Test",
                Request = new
                {
                    Year = "2018",
                    FString="Good Job"
                }
            };
            context.Setup(_smartSqlContext);
            _sqlBuilder.BuildSql(context);
            string sql = "Select T.* From T_Entity_2018 T  Where   T.FString=@FString";
            Assert.Equal<string>(context.RealSql, sql);
        }
        [Fact]
        public void Placeholder_Table_NoneFString_Test()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "Placeholder_Table_Test",
                Request = new
                {
                    Year = "2018"
                }
            };
            context.Setup(_smartSqlContext);
            _sqlBuilder.BuildSql(context);
            string sql = "Select T.* From T_Entity_2018 T";
            Assert.Equal<string>(context.RealSql, sql);
        }
        [Fact]
        public void Placeholder_Table_NoPrepend_Test()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "Placeholder_Table_NoPrepend_Test",
                Request = new
                {
                    Year = "2018"
                }
            };
            context.Setup(_smartSqlContext);
            _sqlBuilder.BuildSql(context);
            string sql = "Select T.* From T_Entity_2018 T";
            Assert.Equal<string>(context.RealSql, sql);
        }
        
    }


}
