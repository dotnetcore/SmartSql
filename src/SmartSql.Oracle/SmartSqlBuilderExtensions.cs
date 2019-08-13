using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using SmartSql.Command;
using SmartSql.Exceptions;

namespace SmartSql
{
    public static class SmartSqlBuilderExtensions
    {
        public static SmartSqlBuilder UseOracleCommandExecuter(this SmartSqlBuilder smartSqlBuilder)
        {
            return UseOracleCommandExecuter(smartSqlBuilder, smartSqlBuilder.LoggerFactory);
        }

        public static SmartSqlBuilder UseOracleCommandExecuter(this SmartSqlBuilder smartSqlBuilder,
            ILoggerFactory loggerFactory)
        {
            var cmdExe = new CommandExecuter(loggerFactory.CreateLogger<CommandExecuter>());
            cmdExe.DbCommandCreated += OnDbCommandCreated;
            smartSqlBuilder.UseCommandExecuter(cmdExe);
            return smartSqlBuilder;
        }

        private static void OnDbCommandCreated(object cmdExe, DbCommandCreatedEventArgs eventArgs)
        {
            var oracleCommand = eventArgs.DbCommand as OracleCommand;
            if (oracleCommand == null)
            {
                throw new SmartSqlException("The ADO.NET Driver is not [Oracle.ManagedDataAccess.Core].");
            }

            oracleCommand.BindByName = true;
            oracleCommand.InitialLONGFetchSize = -1;
        }
    }
}