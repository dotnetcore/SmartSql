namespace SmartSql.Test.Entities
{
    public class Entity
    {
        public Entity()
        {
            
        }
        public Entity(long id)
        {
            Id = id;
        }

        public virtual long Id { get; 
            set; }
        public virtual string Name { get; set; }
        
        
    }
}