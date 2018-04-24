using SmartSql.Abstractions.DbSession;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.Abstractions
{
    public interface IPreparedCommand
    {
        IDbCommand Prepare(IDbConnectionSessionStore sessionStore, RequestContext context);
    }
}
