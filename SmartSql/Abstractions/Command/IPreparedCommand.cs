using SmartSql.Abstractions.DbSession;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.Abstractions.Command
{
    public interface IPreparedCommand
    {
        IDbCommand Prepare(RequestContext context);
    }
}
