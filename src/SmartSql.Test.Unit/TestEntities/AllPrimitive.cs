using System;
using SmartSql.Annotations;

namespace SmartSql.Test.Unit.TestEntities
{
    [System.ComponentModel.DataAnnotations.Schema.Table("T_AllPrimitive")]
    [Scope("CUD_AllPrimitive")]
    [Table("T_AllPrimitive")]
    public class AllPrimitive
    {
        public AllPrimitive() { }

        public AllPrimitive(bool fBoolean, long fInt64)
        {
            Boolean = fBoolean;
            Int64 = fInt64;
        }

        [Annotations.Column(IsPrimaryKey = true, IsAutoIncrement = true)]
        public virtual long Id { get; set; }

        public virtual bool Boolean { get; set; }
        public virtual char Char { get; set; }
        public virtual short Int16 { get; set; }
        public virtual int Int32 { get; set; }
        public virtual long Int64 { get; set; }
        public virtual float Single { get; set; }
        public virtual decimal Decimal { get; set; }
        public virtual DateTime DateTime { get; set; }
        public virtual string String { get; set; } = string.Empty;
        public virtual Guid Guid { get; set; }
        public virtual TimeSpan TimeSpan { get; set; }
        public virtual NumericalEnum NumericalEnum { get; set; }
        public virtual bool? NullableBoolean { get; set; }
        public virtual char? NullableChar { get; set; }
        public virtual short? NullableInt16 { get; set; }
        public virtual int? NullableInt32 { get; set; }
        public virtual long? NullableInt64 { get; set; }
        public virtual float? NullableSingle { get; set; }
        public virtual decimal? NullableDecimal { get; set; }
        public virtual DateTime? NullableDateTime { get; set; }
        public virtual string NullableString { get; set; }
        public virtual Guid? NullableGuid { get; set; }
        public virtual TimeSpan? NullableTimeSpan { get; set; }
        public virtual NumericalEnum? NullableNumericalEnum { get; set; }
    }

    public enum NumericalEnum : byte
    {
        One = 1,
        Two = 2
    }
}
