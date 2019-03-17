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
        public long Id { get; set; }
        public Boolean Boolean { get; set; }
        public Char Char { get; set; }
        //public SByte SByte { get; set; }
        public Byte Byte { get; set; }
        public Int16 Int16 { get; set; }
        //public UInt16 UInt16 { get; set; }
        public Int32 Int32 { get; set; }
        //public UInt32 UInt32 { get; set; }
        public Int64 Int64 { get; set; }
        //public UInt64 UInt64 { get; set; }
        public Single Single { get; set; }
        public Double Double { get; set; }
        public Decimal Decimal { get; set; }
        public DateTime DateTime { get; set; }
        public String String { get; set; }
        #region Other
        public Guid Guid { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public NumericalEnum NumericalEnum { get; set; }
        #endregion

        #region Nullable

        public Boolean? NullableBoolean { get; set; }
        public Char? NullableChar { get; set; }
        //public SByte? NullableSByte { get; set; }
        public Byte? NullableByte { get; set; }
        public Int16? NullableInt16 { get; set; }
        //public UInt16? NullableUInt16 { get; set; }
        public Int32? NullableInt32 { get; set; }
        //public UInt32? NullableUInt32 { get; set; }
        public Int64? NullableInt64 { get; set; }
        //public UInt64? NullableUInt64 { get; set; }
        public Single? NullableSingle { get; set; }
        public Double? NullableDouble { get; set; }
        public Decimal? NullableDecimal { get; set; }
        public DateTime? NullableDateTime { get; set; }
        public String NullableString { get; set; }
        #region Other
        public Guid? NullableGuid { get; set; }
        public TimeSpan? NullableTimeSpan { get; set; }
        public NumericalEnum? NullableNumericalEnum { get; set; }
        #endregion

        #endregion
    }


}
