using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Abstractions
{
    /// <summary>
    /// Sql 请求上下文
    /// </summary>
    public interface IRequestContext
    {
        String Scope { get; set; }
        String SqlId { get; set; }
        String FullSqlId { get; }
        Object Request { get; set; }
    }

    public class RequestContext : IRequestContext
    {
        public String Scope { get; set; }
        public String SqlId { get; set; }
        public String FullSqlId { get { return $"{Scope}.{SqlId}"; } }
        public Object Request { get; set; }
    }
}
