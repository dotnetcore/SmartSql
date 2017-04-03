using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Abstractions.DbSession
{
    /// <summary>
    /// 数据库链接会话存储
    /// </summary>
    public interface IDbConnectionSessionStore : IDisposable
    {
        IDbConnectionSession LocalSession { get; }
        void Store(IDbConnectionSession session);
    }
}
