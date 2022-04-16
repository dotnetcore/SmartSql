using System;

namespace SmartSql.Test.Unit.Cache
{
    public class CachedEntity
    {
        public string Name { get; set; }

        protected bool Equals(CachedEntity other)
        {
            return Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CachedEntity)obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}