using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.DataSource;
namespace SmartSql.DbSession
{
    public interface IDbSessionStore : IDisposable
    {
        IDbSession LocalSession { get; }
        IDbSession Open();
    }
}
