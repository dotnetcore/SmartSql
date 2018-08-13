using SmartSql.Configuration.Statements;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Utils
{
    public class SqlCommandAnalyzer
    {
        public SqlCommandType Analyse(string realSql)
        {
            SqlCommandType commandType = SqlCommandType.Unknown;
            var statements = realSql.Trim().Replace("\r\n", " ").Trim().Split(';');
            foreach (var statement in statements)
            {
                var statementStr = statement.TrimStart(' ', '(');
                if (statementStr.StartsWith("Insert", StringComparison.CurrentCultureIgnoreCase))
                {
                    commandType = commandType | SqlCommandType.Insert;
                }
                if (statementStr.StartsWith("Delete", StringComparison.CurrentCultureIgnoreCase))
                {
                    commandType = commandType | SqlCommandType.Delete;
                }
                if (statementStr.StartsWith("Update", StringComparison.CurrentCultureIgnoreCase))
                {
                    commandType = commandType | SqlCommandType.Update;
                }
                if (statementStr.StartsWith("Select", StringComparison.CurrentCultureIgnoreCase))
                {
                    commandType = commandType | SqlCommandType.Select;
                }
            }
            return commandType;
        }
    }
}
