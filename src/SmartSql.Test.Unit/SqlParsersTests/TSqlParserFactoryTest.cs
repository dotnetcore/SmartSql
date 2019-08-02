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
            var insertStatementListener = new DMLStatementListener("@");
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
            Assert.Equal("T_User", insertStatementListener.Insert.Table);
        }


        public class DMLStatementListener : TSqlParserBaseListener
        {
            private readonly string _parameterPrefix;

            public DMLStatementListener(string parameterPrefix)
            {
                _parameterPrefix = parameterPrefix;
            }

            public Insert Insert { get; private set; }

            public override void EnterInsert_statement(TSqlParser.Insert_statementContext context)
            {
                Insert = new Insert
                {
                    ParameterPrefix = _parameterPrefix,
                    Table = context.ddl_object().full_table_name().id().First().simple_id().GetText(),
                    Columns = new List<string>(),
                    Parameters = new List<string>()
                };

                foreach (var idContext in context.column_name_list().id())
                {
                    Insert.Columns.Add(idContext.GetText());
                }
            }

            public override void EnterInsert_statement_value(TSqlParser.Insert_statement_valueContext context)
            {
                foreach (var expr in context.table_value_constructor().expression_list().First().expression())
                {
                    var paramName = expr.primitive_expression().GetText();
                    if (paramName.StartsWith(_parameterPrefix))
                    {
                        paramName = paramName.Substring(_parameterPrefix.Length);
                    }

                    Insert.Parameters.Add(paramName);
                }
            }
        }
    }
}