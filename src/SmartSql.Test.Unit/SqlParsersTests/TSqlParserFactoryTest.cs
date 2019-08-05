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
            var dmlStatementListener = new DMLStatementListener("@");
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
            TSqlParserFactory.Parse(insertSql, dmlStatementListener);
            Assert.Equal("T_User", dmlStatementListener.Current.Table);
        }

        [Fact]
        public void Update()
        {
            var dmlStatementListener = new DMLStatementListener("@");
            var updateSql = @"UPDATE T_User
            SET
                UserName = @UserName,
                Status = @Status
                WHERE Id=@Id";

            TSqlParserFactory.Parse(updateSql, dmlStatementListener);
            Assert.Equal("T_User", dmlStatementListener.Current.Table);
        }


        public class DMLStatementListener : TSqlParserBaseListener
        {
            private readonly string _parameterPrefix;

            public DMLStatementListener(string parameterPrefix)
            {
                _parameterPrefix = parameterPrefix;
                DMLClause = new DMLClause();
            }

            public DMLClause DMLClause { get; set; }

            public IStatement Current { get; set; }

            #region Insert

            public override void EnterInsert_statement(TSqlParser.Insert_statementContext context)
            {
                var insert = new Insert
                {
                    ParameterPrefix = _parameterPrefix,
                    Table = context.ddl_object().full_table_name().id().First().simple_id().GetText(),
                    Columns = new List<string>(),
                    Parameters = new List<string>()
                };
                Current = insert;
                DMLClause.Statements.Add(Current);
                foreach (var idContext in context.column_name_list().id())
                {
                    insert.Columns.Add(idContext.GetText());
                }
            }

            public override void EnterInsert_statement_value(TSqlParser.Insert_statement_valueContext context)
            {
                var insert = Current as Insert;
                foreach (var expr in context.table_value_constructor().expression_list().First().expression())
                {
                    var paramName = expr.primitive_expression().GetText();
                    if (paramName.StartsWith(_parameterPrefix))
                    {
                        paramName = paramName.Substring(_parameterPrefix.Length);
                    }

                    insert.Parameters.Add(paramName);
                }
            }

            #endregion

            #region Update

            public override void EnterUpdate_statement(TSqlParser.Update_statementContext context)
            {
                var update = new Update
                {
                    Table = context.ddl_object().full_table_name().table.GetText(),
                };
                Current = update;
            }

            public override void EnterUpdate_elem(TSqlParser.Update_elemContext context)
            {
                var columnName = context.full_column_name().id().GetText();
                var paramName = context.expression().primitive_expression().GetText();
            }

            public override void EnterSearch_condition_list(TSqlParser.Search_condition_listContext context)
            {
                context.search_condition().First().search_condition_and().First().search_condition_not().First().predicate().comparison_operator().GetText()
            }

            #endregion

            #region Delete

            public override void EnterDelete_statement(TSqlParser.Delete_statementContext context)
            {
                var delete = new Delete
                {
                    Table = context.table_sources().table_source().First().GetText()
                };
                Current = delete;
                base.EnterDelete_statement(context);
            }

            public override void EnterDelete_statement_from(TSqlParser.Delete_statement_fromContext context)
            {
            }

            #endregion
        }
    }
}