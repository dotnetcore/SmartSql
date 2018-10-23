using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Utils;
using Xunit;
using System.Linq;
using System.Text.RegularExpressions;

namespace SmartSql.UTests.Utils
{
    public class SqlParamAnalyzer_Tests
    {
        SqlParamAnalyzer _sqlParamAnalyzer = new SqlParamAnalyzer(true, "@");

        [Fact]
        public void Analyse()
        {
            var paramStrs = _sqlParamAnalyzer.Analyse("@Good,@Yes.Good").ToArray();


        }
        [Fact]
        public void IN()
        {
            var reg = new Regex(@"[i|I][n|N]\s*[@]([\p{L}\p{N}_.]+)");

            var ms = reg.Matches(@"select In  
@good,@good.job");
        }
    }
}
