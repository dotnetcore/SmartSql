using SmartSql.Abstractions.DbSession;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Abstractions
{
    public interface ISession
    {
        IDbConnectionSession BeginSession(RequestContext context);
        void EndSession();
    }
}
