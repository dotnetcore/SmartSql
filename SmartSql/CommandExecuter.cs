using SmartSql.Abstractions;
using SmartSql.Abstractions.DbSession;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql
{
    public class CommandExecuter : ICommandExecuter
    {
        private IDbConnectionSessionStore sessionStore;
        private readonly IPreparedCommand preparedCommand;
        public CommandExecuter(IDbConnectionSessionStore sessionStore, IPreparedCommand  preparedCommand)
        {
            this.sessionStore = sessionStore;
            this.preparedCommand = preparedCommand;
        }

        public int ExecuteNonQuery(RequestContext context)
        {
            var dbCommand = preparedCommand.Prepare(sessionStore, context);
            return dbCommand.ExecuteNonQuery();
        }

        public IDataReader ExecuteReader(RequestContext context)
        {
            var dbCommand = preparedCommand.Prepare(sessionStore, context);
            return dbCommand.ExecuteReader();
        }

        public IDataReader ExecuteReader(RequestContext context, CommandBehavior behavior)
        {
            var dbCommand = preparedCommand.Prepare(sessionStore, context);
            return dbCommand.ExecuteReader(behavior);
        }

        public object ExecuteScalar(RequestContext context)
        {
            var dbCommand = preparedCommand.Prepare(sessionStore, context);
            return dbCommand.ExecuteScalar();
        }
    }
}
