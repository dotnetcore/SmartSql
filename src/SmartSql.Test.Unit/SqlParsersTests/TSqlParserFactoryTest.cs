using SmartSql.SqlParsers;
using SmartSql.SqlParsers.TSql;
using Xunit;

namespace SmartSql.Test.Unit.SqlParsersTests
{
    public class TSqlParserFactoryTest
    {
        [Fact]
        public void Insert()
        {
            var insertStatementListener = new InsertStatementListener();
            var insertSql = @"Insert Into T_User
            (
            UserName,
            Status
            )
            VALUES
            (
            @UserName,
            @Status
            )";
            TSqlParserFactory.Parse(insertSql, insertStatementListener);
        }


        public class InsertStatementListener : TSqlParserBaseListener
        {
            public string ColumnNames { get; set; }

            public override void EnterDml_clause(TSqlParser.Dml_clauseContext context)
            {
                
                base.EnterDml_clause(context);
            }

            public override void EnterInsert_statement(TSqlParser.Insert_statementContext context)
            {
                ColumnNames = context.column_name_list().GetText();
            }

            public override void EnterInsert_statement_value(TSqlParser.Insert_statement_valueContext context)
            {
                base.EnterInsert_statement_value(context);
            }

            public override void EnterInsert_with_table_hints(TSqlParser.Insert_with_table_hintsContext context)
            {
                base.EnterInsert_with_table_hints(context);
            }
        }
    }
}