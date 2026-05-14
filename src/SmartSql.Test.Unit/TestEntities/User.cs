using SmartSql.Annotations;

namespace SmartSql.Test.Unit.TestEntities
{
    public class User
    {
        public User() { }

        public User(long id) { Id = id; }

        public User(long id, string name) { Id = id; UserName = name; }

        public User(long id, string name, UserStatus status)
        {
            Id = id;
            UserName = name;
            Status = status;
        }

        [Column("id")]
        public virtual long Id { get; set; }
        [Column("user_name")]
        public virtual string UserName { get; set; }
        [Column("status")]
        public virtual UserStatus Status { get; set; }
        [Column("is_delete")]
        public virtual bool IsDelete { get; set; }
    }

    public enum UserStatus : short
    {
        Ok = 1
    }
}
