namespace SmartSql.Test.Unit.TestEntities
{
    public class User
    {
        public User() { }

        public User(long id) { Id = id; }

        public User(long id, string name) { Id = id; UserName = name; }

        public virtual long Id { get; set; }
        public virtual string UserName { get; set; }
    }
}
