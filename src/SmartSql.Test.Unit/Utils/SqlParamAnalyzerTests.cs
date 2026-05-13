using System.Linq;
using FluentAssertions;
using SmartSql.Utils;
using Xunit;

namespace SmartSql.Test.Unit.Utils
{
    public class SqlParamAnalyzerTests
    {
        private SqlParamAnalyzer _sqlParamAnalyzer;

        public SqlParamAnalyzerTests()
        {
            _sqlParamAnalyzer = new SqlParamAnalyzer(false, "@");
        }

        [Fact]
        public void Should_ReturnEmptyResult_When_SqlHasNoParameters()
        {
            var result = _sqlParamAnalyzer.Analyse("Sp_QueryByPage");

            result.Should().BeEmpty();
        }

        [Fact]
        public void Should_ExtractParameters_When_SqlHasParameters()
        {
            var result = _sqlParamAnalyzer.Analyse("Select * from t_user where name = @name;");

            result.Count.Should().Be(1);
            result.First().Should().Be("name");
        }
    }
}
