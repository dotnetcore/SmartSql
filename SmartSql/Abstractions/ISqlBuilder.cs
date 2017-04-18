using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Abstractions
{
    /// <summary>
    /// Sql 生成器
    /// </summary>
    public interface ISqlBuilder
    {
        String BuildSql(RequestContext context);
    }
}
