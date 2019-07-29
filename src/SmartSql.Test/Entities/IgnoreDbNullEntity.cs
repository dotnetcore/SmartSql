using System;
using System.Reflection;
using System.Threading.Tasks;

namespace SmartSql.Test.Entities
{
    [Annotations.Table("T_IgnoreDbNullEntity")]
    public class IgnoreDbNullEntity
    {
        public long Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual long DbNullId { get; set; }
    }
}