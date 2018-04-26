using SmartSql.Abstractions;
using SmartSql.Abstractions.Command;
using SmartSql.Abstractions.DbSession;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql
{
    public class CommandExecuter : ICommandExecuter
    {
        private readonly IPreparedCommand _preparedCommand;
        public CommandExecuter(IPreparedCommand preparedCommand)
        {
            _preparedCommand = preparedCommand;
        }

        public int ExecuteNonQuery(RequestContext context)
        {
            var dbCommand = _preparedCommand.Prepare(context);
            return dbCommand.ExecuteNonQuery();
        }

        public IDataReader ExecuteReader(RequestContext context)
        {
            var dbCommand = _preparedCommand.Prepare(context);
            return dbCommand.ExecuteReader();
        }

        public IDataReader ExecuteReader(RequestContext context, CommandBehavior behavior)
        {
            var dbCommand = _preparedCommand.Prepare(context);
            return dbCommand.ExecuteReader(behavior);
        }

        public object ExecuteScalar(RequestContext context)
        {
            var dbCommand = _preparedCommand.Prepare(context);
            return dbCommand.ExecuteScalar();
        }
    }
}
