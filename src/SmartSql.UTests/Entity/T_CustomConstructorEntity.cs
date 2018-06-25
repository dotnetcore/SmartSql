using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.UTests.Entity
{
    public class T_CustomConstructorEntity
    {
        public T_CustomConstructorEntity(long fLong, decimal fDecimal)
        {
            FLong = fLong;
            FDecimal = fDecimal;
        }

        /// <summary>
        /// FLong
        /// </summary>		
        public virtual long FLong { get; }

        /// <summary>
        /// FDecimal
        /// </summary>		
        public virtual decimal FDecimal { get; }
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
        ///public virtual DateTime FTimestamp { get; set; }

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

        public virtual String JustMeString { get; set; }
    }
}
