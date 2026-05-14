using FluentAssertions;
using SmartSql.InvokeSync.Utils;
using Xunit;

namespace SmartSql.Test.Unit.Utils
{
    public class InsertWithIdTests
    {
        private InsertWithId _insertWithId;

        public InsertWithIdTests()
        {
            _insertWithId = new InsertWithId();
        }

        [Fact]
        public void Should_ReplaceIdPlaceholder_When_SqlContainsId()
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

            var newSql = _insertWithId.Replace(insertSql, "id", "Id", "@");

            newSql.Should().Be(@"Insert Into T_User
            (id,
            UserName,
            Status
            )
            VALUES
            (@Id,
            @UserName,
            @Status
            )
            ;select last_insert_rowid() from T_User");
        }

        [Fact]
        public void Should_ReplaceIdPlaceholder_When_SqlHasColumnList()
        {
            var insertSql = @"Insert Into T_User(
            UserName,
            Status
            )
            VALUES(
            @UserName,
            @Status
            )
            ;select last_insert_rowid() from T_User";

            var newSql = _insertWithId.Replace(insertSql, "id", "Id", "@");

            newSql.Should().Be(@"Insert Into T_User(id,
            UserName,
            Status
            )
            VALUES(@Id,
            @UserName,
            @Status
            )
            ;select last_insert_rowid() from T_User");
        }
    }
}
