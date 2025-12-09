
namespace SmartSql.Test.Entities
{
    public class NestedEntity
    {
        public virtual long Id { get; set; }
        public virtual NestedProperty1 NestedProp1 { get; set; }
    }
    public class NestedProperty1
    {
        public virtual NestedProperty2 NestedProp2 { get; set; }
    }
    public class NestedProperty2
    {
        public virtual string NestedProp3 { get; set; }
    }
}
