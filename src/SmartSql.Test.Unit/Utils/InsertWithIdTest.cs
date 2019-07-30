using SmartSql.InvokeSync.Utils;
using Xunit;

namespace SmartSql.Test.Unit.Utils
{
    public class InsertWithIdTest
    {
        private InsertWithId _insertWithId;

        public InsertWithIdTest()
        {
            _insertWithId = new InsertWithId();
        }

        [Fact]
        public void Replace()
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
            Assert.Equal(@"Insert Into T_User
            (id,
            UserName,
            Status
            )
            VALUES
            (@Id,
            @UserName,
            @Status
            )
            ;select last_insert_rowid() from T_User",newSql);
        }
        [Fact]
        public void Replace2()
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
            Assert.Equal(@"Insert Into T_User(id,
            UserName,
            Status
            )
            VALUES(@Id,
            @UserName,
            @Status
            )
            ;select last_insert_rowid() from T_User",newSql);
        }
    }
}