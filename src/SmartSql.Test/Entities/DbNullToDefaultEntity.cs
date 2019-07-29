using System;
using System.Reflection;
using System.Threading.Tasks;

namespace SmartSql.Test.Entities
{
    [Annotations.Table("T_DbNullToDefaultEntity")]
    public class DbNullToDefaultEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual long DbNullId { get; set; }
    }
}