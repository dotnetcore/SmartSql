using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Abstractions
{
    /// <summary>
    /// Sql 请求上下文
    /// </summary>
    public class RequestContext
    {
        public String Scope { get; set; }
        public String SqlId { get; set; }
        public String FullSqlId => $"{Scope}.{SqlId}";
        public Object Request { get; set; }

        /// <summary>
        /// 一次请求上下文共享的数据字典
        /// </summary>
        public IDictionary Items { get; } = new Dictionary<Object, Object>();

        public Object Response { get; set; }
    }
}
