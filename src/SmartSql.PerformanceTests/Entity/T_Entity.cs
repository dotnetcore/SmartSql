using Chloe.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SmartSql.PerformanceTests
{
    /// <summary>
    ///T_Entity
    /// </summary>	
    [System.ComponentModel.DataAnnotations.Schema.Table("T_Entity")]
    public class T_Entity
    {
        /// <summary>
        /// FLong
        /// </summary>		
        [Column(IsPrimaryKey = true)]
        [Key]
        public virtual long FLong { get; set; }

        /// <summary>
        /// FString
        /// </summary>		
        public virtual string FString { get; set; }

        /// <summary>
        /// FDecimal
        /// </summary>		
        public virtual decimal FDecimal { get; set; }

        /// <summary>
        /// FNullDecimal
        /// </summary>		
        public virtual decimal? FNullDecimal { get; set; }

        /// <summary>
        /// FBool
        /// </summary>		
        public virtual bool FBool { get; set; }

        /// <summary>
        /// FNullBool
        /// </summary>		
        public virtual bool? FNullBool { get; set; }

        /// <summary>
        /// FTimestamp
        /// </summary>		
        //public virtual byte[] FTimestamp { get; set; }

        /// <summary>
        /// Status
        /// </summary>		
        public virtual EntityStatus Status { get; set; }

        /// <summary>
        /// NullStatus
        /// </summary>		
        public virtual EntityStatus? NullStatus { get; set; }

        /// <summary>
        /// CreationTime
        /// </summary>		
        public virtual DateTime CreationTime { get; set; }

        /// <summary>
        /// LastUpdateTime
        /// </summary>		
        public virtual DateTime? LastUpdateTime { get; set; }


    }

    public enum EntityStatus : short
    {
        Ok = 1
    }
}
