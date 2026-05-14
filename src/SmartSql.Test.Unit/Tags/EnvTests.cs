using FluentAssertions;
using SmartSql.Configuration;
using SmartSql.Configuration.Tags;
using SmartSql.Data;
using SmartSql.DataSource;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    public class EnvTests
    {
        private RequestContext CreateContext(string dbProviderName)
        {
            var sqlParams = new SqlParameterCollection();
            var context = new RequestContext
            {
                Request = sqlParams
            };
            context.SetupParameters();

            context.ExecutionContext = new ExecutionContext
            {
                SmartSqlConfig = new SmartSqlConfig
                {
                    Database = new Database
                    {
                        DbProvider = new DbProvider
                        {
                            Name = dbProviderName
                        }
                    }
                }
            };

            return context;
        }

        [Fact]
        public void Should_ReturnTrue_When_DbProviderMatches()
        {
            var tag = new Env { DbProvider = "MySql" };
            var context = CreateContext("MySql");

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_DbProviderNotMatches()
        {
            var tag = new Env { DbProvider = "MySql" };
            var context = CreateContext("SqlServer");

            var result = tag.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnTrue_When_DbProviderMatches_PostgreSql()
        {
            var tag = new Env { DbProvider = "PostgreSql" };
            var context = CreateContext("PostgreSql");

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }
    }
}
