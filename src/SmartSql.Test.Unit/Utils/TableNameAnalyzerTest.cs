using System;
using System.Text.RegularExpressions;
using SmartSql.Configuration;
using SmartSql.InvokeSync;
using SmartSql.InvokeSync.Utils;
using Xunit;

namespace SmartSql.Test.Unit.Utils
{
    public class TableNameAnalyzerTest
    {
        private TableNameAnalyzer _tableNameAnalyzer;

        public TableNameAnalyzerTest()
        {
            _tableNameAnalyzer = new TableNameAnalyzer();
        }

        [Fact]
        public void Insert()
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
                if (tableName == "T_User")
                {
                    return operation + " new_user";
                }

                throw new ArgumentException();
            });
        }

        [Fact]
        public void Update()
        {
            var updateSql = @"UPDATE T_User
            Set
                UserName = @UserName
                Status = @Status
                Where Id=@Id";
            var newSql = _tableNameAnalyzer.Replace(StatementType.Update, updateSql, (tableName, operation) =>
            {
                if (tableName == "T_User")
                {
                    return operation + " new_user";
                }

                throw new ArgumentException();
            });
        }

        [Fact]
        public void Delete()
        {
            var updateSql = @"Delete T_User
      Where Id=@Id";
            var newSql = _tableNameAnalyzer.Replace(StatementType.Delete, updateSql, (tableName, operation) =>
            {
                if (tableName == "T_User")
                {
                    return operation + " new_user";
                }

                throw new ArgumentException();
            });
        }

        [Fact]
        public void DeleteFrom()
        {
            var updateSql = @"Delete From T_User
      Where Id=@Id";
            var newSql = _tableNameAnalyzer.Replace(StatementType.Delete, updateSql, (tableName, operation) =>
            {
                if (tableName == "T_User")
                {
                    return operation + " new_user";
                }

                throw new ArgumentException();
            });
        }
    }
}