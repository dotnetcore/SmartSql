namespace SmartSql.Test.Entities
{
    public class Entity : EntityBase
    {
        public Entity()
        {
        }

        public Entity(long id)
        {
            Id = id;
        }

        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
    }

    public abstract class EntityBase
    {
        public virtual bool Deleted { get; set; }
    }
}