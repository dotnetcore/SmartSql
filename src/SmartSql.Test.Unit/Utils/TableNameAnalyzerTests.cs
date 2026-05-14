using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using SmartSql.Configuration;
using SmartSql.InvokeSync;
using SmartSql.InvokeSync.Utils;
using Xunit;

namespace SmartSql.Test.Unit.Utils
{
    public class TableNameAnalyzerTests
    {
        private TableNameAnalyzer _tableNameAnalyzer;

        public TableNameAnalyzerTests()
        {
            _tableNameAnalyzer = new TableNameAnalyzer();
        }

        [Fact]
        public void Should_ReplaceTableName_When_InsertStatement()
        {
            var insertSql = @"Insert Into T_User
            (
            UserName,
            Status
            )
            VALUES
            (
            @UserName,
            @Status
            )
            ;select last_insert_rowid() from T_User";

            var newSql = _tableNameAnalyzer.Replace(StatementType.Insert, insertSql, (tableName, operation) =>
            {
                return operation + "new_user";
            });

            newSql.Should().Be(@"Insert Into new_user
            (
            UserName,
            Status
            )
            VALUES
            (
            @UserName,
            @Status
            )
            ;select last_insert_rowid() from T_User");
        }

        [Fact]
        public void Should_ReplaceTableName_When_UpdateStatement()
        {
            var updateSql = @"UPDATE T_User
            Set
                UserName = @UserName
                Status = @Status
                Where Id=@Id";

            var newSql = _tableNameAnalyzer.Replace(StatementType.Update, updateSql, (tableName, operation) =>
            {
                return operation + "new_user";
            });

            newSql.Should().Be(@"UPDATE new_user
            Set
                UserName = @UserName
                Status = @Status
                Where Id=@Id");
        }

        [Fact]
        public void Should_ReplaceTableName_When_DeleteStatement()
        {
            var updateSql = @"Delete T_User
      Where Id=@Id";

            var newSql = _tableNameAnalyzer.Replace(StatementType.Delete, updateSql, (tableName, operation) =>
            {
                return operation + "new_user";
            });

            newSql.Should().Be(@"Delete new_user
      Where Id=@Id");
        }

        [Fact]
        public void Should_ReplaceTableName_When_DeleteFromStatement()
        {
            var updateSql = @"Delete From T_User
      Where Id=@Id";

            var newSql = _tableNameAnalyzer.Replace(StatementType.Delete, updateSql, (tableName, operation) =>
            {
                return operation + "new_user";
            });

            newSql.Should().Be(@"Delete From new_user
      Where Id=@Id");
        }
    }
}
