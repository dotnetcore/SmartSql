using System;

namespace SmartSql.DyRespository
{
    public class SqlMapAttribute : Attribute
    {
        public string Scope { get; set; }
    }
}