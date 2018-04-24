using SmartSql.Abstractions;
using SmartSql.Abstractions.DbSession;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql
{
    public class PreparedCommand : IPreparedCommand
    {
        public IDbCommand Prepare(IDbConnectionSessionStore sessionStore, RequestContext context)
        {
            var dbCommand = sessionStore.LocalSession.Connection.CreateCommand();
            throw new NotImplementedException();
        }
    }
}
