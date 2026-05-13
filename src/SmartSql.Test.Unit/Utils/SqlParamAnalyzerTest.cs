using System.Linq;
using SmartSql.Utils;
using Xunit;

namespace SmartSql.Test.Unit.Utils
{
    public class SqlParamAnalyzerTest
    {
        private SqlParamAnalyzer _sqlParamAnalyzer;

        public SqlParamAnalyzerTest()
        {
            _sqlParamAnalyzer = new SqlParamAnalyzer(false, "@");
        }

        [Fact]
        public void Analyse_NonParam()
        {
            var result = _sqlParamAnalyzer.Analyse("Sp_QueryByPage");
            Assert.Empty(result);
        }

        [Fact]
        public void Analyse()
        {
            var result = _sqlParamAnalyzer.Analyse("Select * from t_user where name = @name;");
            Assert.Equal(1, result.Count);
            Assert.Equal("name", result.First());
        }
    }
}