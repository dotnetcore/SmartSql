using System;

namespace SmartSql.DyRepository
{
    public class SqlMapAttribute : Attribute
    {
        public string Scope { get; set; }
    }
}