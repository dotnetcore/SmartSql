using System;
using SmartSql.Annotations;

namespace SmartSql.Test.Entities
{
    [System.ComponentModel.DataAnnotations.Schema.Table("T_AllPrimitive")]
    [Scope("CUD_AllPrimitive")]
    [Table("T_AllPrimitive")]
    public class AllPrimitive
    {
        public AllPrimitive()
        {
        }

        public AllPrimitive(Boolean fBoolean, Int64 fInt64)
        {
            Boolean = fBoolean;
            Int64 = fInt64;
        }

        [Annotations.Column(IsPrimaryKey = true, IsAutoIncrement = true)]
        public virtual long Id { get; set; }

        public virtual Boolean Boolean { get; set; }

        public virtual Char Char { get; set; }

        public virtual Int16 Int16 { get; set; }
        public virtual Int32 Int32 { get; set; }
        public virtual Int64 Int64 { get; set; }

        public virtual Single Single { get; set; }

        public virtual Decimal Decimal { get; set; }
        public virtual DateTime DateTime { get; set; }
        public virtual String String { get; set; } = String.Empty;

        #region Other

        public virtual Guid Guid { get; set; }
        public virtual TimeSpan TimeSpan { get; set; }
        public virtual NumericalEnum NumericalEnum { get; set; }

        #endregion

        #region Nullable

        public virtual Boolean? NullableBoolean { get; set; }

        public virtual Char? NullableChar { get; set; }

        public virtual Int16? NullableInt16 { get; set; }

        public virtual Int32? NullableInt32 { get; set; }

        public virtual Int64? NullableInt64 { get; set; }

        public virtual Single? NullableSingle { get; set; }
        public virtual Decimal? NullableDecimal { get; set; }
        public virtual DateTime? NullableDateTime { get; set; }
        public virtual String NullableString { get; set; }

        #region Other

        public virtual Guid? NullableGuid { get; set; }
        public virtual TimeSpan? NullableTimeSpan { get; set; }
        public virtual NumericalEnum? NullableNumericalEnum { get; set; }

        #endregion

        #endregion
    }
}