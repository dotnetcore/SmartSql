using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using SmartSql.Configuration;

namespace SmartSql.Utils
{
    public class StatementAnalyzer
    {
        public StatementType Analyse(string realSql)
        {
            return CacheUtil<StatementAnalyzer, String, StatementType>.GetOrAdd(realSql, AnalyseImpl);
        }

        private StatementType AnalyseImpl(string realSql)
        {
            StatementType statementType = StatementType.Unknown;
            var statements = realSql.Trim().Split(';');
            foreach (var statement in statements)
            {
                if (String.IsNullOrEmpty(statement)) { continue; }
                var statementStr = statement.Trim().TrimStart('(');
                if (statementStr.StartsWith("Insert", StringComparison.CurrentCultureIgnoreCase))
                {
                    statementType = statementType | StatementType.Insert;
                }
                if (statementStr.StartsWith("Delete", StringComparison.CurrentCultureIgnoreCase))
                {
                    statementType = statementType | StatementType.Delete;
                }
                if (statementStr.StartsWith("Update", StringComparison.CurrentCultureIgnoreCase))
                {
                    statementType = statementType | StatementType.Update;
                }
                if (statementStr.StartsWith("Select", StringComparison.CurrentCultureIgnoreCase))
                {
                    statementType = statementType | StatementType.Select;
                }
            }
            return statementType;
        }
    }
}
