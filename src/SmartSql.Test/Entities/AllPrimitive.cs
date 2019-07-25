using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSql.Test.Entities
{
    [Table("T_AllPrimitive")]
    [Annotations.Table("T_AllPrimitive")]
    public class AllPrimitive
    {
        public AllPrimitive()
        {
            //System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute
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

        //public SByte SByte { get; set; }
        public virtual Byte Byte { get; set; }

        public virtual Int16 Int16 { get; set; }

        //public UInt16 UInt16 { get; set; }
        public virtual Int32 Int32 { get; set; }

        //public UInt32 UInt32 { get; set; }
        public virtual Int64 Int64 { get; set; }

        //public UInt64 UInt64 { get; set; }
        public virtual Single Single { get; set; }

        //public Double Double { get; set; }
        public virtual Decimal Decimal { get; set; }
        public virtual DateTime DateTime { get; set; }
        public virtual String String { get; set; }

        #region Other

        public virtual Guid Guid { get; set; }
        public virtual TimeSpan TimeSpan { get; set; }
        public virtual NumericalEnum NumericalEnum { get; set; }

        #endregion

        #region Nullable

        public virtual Boolean? NullableBoolean { get; set; }

        public virtual Char? NullableChar { get; set; }

        //public SByte? NullableSByte { get; set; }
        public virtual Byte? NullableByte { get; set; }

        public virtual Int16? NullableInt16 { get; set; }

        //public UInt16? NullableUInt16 { get; set; }
        public virtual Int32? NullableInt32 { get; set; }

        //public UInt32? NullableUInt32 { get; set; }
        public virtual Int64? NullableInt64 { get; set; }

        //public UInt64? NullableUInt64 { get; set; }
        public virtual Single? NullableSingle { get; set; }
        public virtual Double? NullableDouble { get; set; }
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