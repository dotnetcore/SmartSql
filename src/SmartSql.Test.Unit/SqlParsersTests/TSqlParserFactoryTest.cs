using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SmartSql.DML;
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
            Assert.Equal("T_User",insertStatementListener.TableName);
        }


        public class InsertStatementListener : TSqlParserBaseListener
        {
            public String TableName { get; set; }
            public IList<String> Columns { get; set; }
            public IList<String> Values { get; set; }

            public override void EnterInsert_statement(TSqlParser.Insert_statementContext context)
            {
                TableName = context.ddl_object().full_table_name().id().First().simple_id().GetText();
                Columns = context.column_name_list().GetText().Split(",");
            }

            public override void EnterInsert_statement_value(TSqlParser.Insert_statement_valueContext context)
            {
                Values = new List<string>();
                foreach (var expr in context.table_value_constructor().expression_list().First().expression())
                {
                    Values.Add(expr.primitive_expression().GetText());
                }
            }

            public override void EnterInsert_with_table_hints(TSqlParser.Insert_with_table_hintsContext context)
            {
                base.EnterInsert_with_table_hints(context);
            }
        }
    }
}