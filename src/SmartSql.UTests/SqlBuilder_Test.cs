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
        SmartSqlOptions _smartSqlOptions;
        public SqlBuilder_Test()
        {
            _sqlBuilder = new SqlBuilder(LoggerFactory.CreateLogger<SqlBuilder>());
            _smartSqlOptions = new SmartSqlOptions
            {
                SqlBuilder = _sqlBuilder
            };
            _smartSqlOptions.Setup();
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
            context.Setup(_smartSqlOptions);

            string sql = @"Select T.* From T_Entity T
      Order By Id Desc";
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
            context.Setup(_smartSqlOptions);
            string sql = @"Select T.* From T_Entity T
      Order By Id Desc";
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
                    FString = "Good Job"
                }
            };
            context.Setup(_smartSqlOptions);

            string sql = @"Select T.* From T_Entity_2018 T
       Where   
          T.FString=@FString";
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
            context.Setup(_smartSqlOptions);
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
            context.Setup(_smartSqlOptions);
            string sql = "Select T.* From T_Entity_2018 T";
            Assert.Equal<string>(context.RealSql, sql);
        }

    }


}
