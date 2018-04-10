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
        private IDbCommand dbCommand;
        public CommandExecuter(IDbConnectionSessionStore sessionStore)
        {
            this.sessionStore = sessionStore;
            dbCommand = sessionStore.LocalSession.Connection.CreateCommand();
            dbCommand.Transaction = sessionStore.LocalSession.Transaction;
        }



        public int ExecuteNonQuery()
        {

            throw new NotImplementedException();
        }

        public IDataReader ExecuteReader()
        {
            throw new NotImplementedException();
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            throw new NotImplementedException();
        }

        public object ExecuteScalar()
        {
            throw new NotImplementedException();
        }
    }
}
