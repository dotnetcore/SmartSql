using System;

namespace SmartSql.DyRepository.Annotations
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    public class SqlMapAttribute : Attribute
    {
        /// <summary>
        /// SmartSqlMapConfig.Scope 映射
        /// </summary>
        public string Scope { get; set; }
    }
}