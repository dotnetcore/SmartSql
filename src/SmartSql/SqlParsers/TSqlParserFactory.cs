using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using SmartSql.SqlParsers.TSql;

namespace SmartSql.SqlParsers
{
    public class TSqlParserFactory
    {
        public static void Parse(string sql, IParseTreeListener listener)
        {
            var stream = CharStreams.fromstring(sql);
            var lexer = new TSqlLexer(new CaseChangingCharStream(stream, true));
            var tokenStream = new CommonTokenStream(lexer);
            var context = new TSqlParser(tokenStream)
            {
                BuildParseTree = true
            }.tsql_file();
            ParseTreeWalker.Default.Walk(listener, context);
        }
    }
}