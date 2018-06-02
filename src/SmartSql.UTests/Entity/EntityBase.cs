using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.UTests.Entity
{
    public class EntityBase
    {
        public long Id { get; set; }
        /// <summary>
        /// FString
        /// </summary>		
        public virtual string FString { get; set; }
    }
}
