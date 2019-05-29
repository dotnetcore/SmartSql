using System;
using System.Linq;
using SmartSql.Configuration;

namespace SmartSql.InvokeSync
{
    public class SyncFilter : ISyncFilter
    {
        private readonly SyncFilterOptions _filterOptions;

        public SyncFilter(SyncFilterOptions filterOptions)
        {
            _filterOptions = filterOptions;
        }

        public bool Filter(ExecutionContext executionContext)
        {
            var reqContext = executionContext.Request;

            #region Ignore

            if (
                _filterOptions.IgnoreStatementType.HasValue
                &&(_filterOptions.IgnoreStatementType & reqContext.Statement.StatementType)
                != StatementType.Unknown)
            {
                return false;
            }

            if (_filterOptions.IgnoreScopes != null 
                && _filterOptions.IgnoreScopes.Any()
                && _filterOptions.IgnoreScopes.Any(scope =>
                    scope.Equals(reqContext.Scope, StringComparison.OrdinalIgnoreCase))
                )
            {
                return false;
            }

            if (_filterOptions.IgnoreSqlIds != null 
                && _filterOptions.IgnoreSqlIds.Any()
                && _filterOptions.IgnoreSqlIds.Any(sqlId =>
                    sqlId.Equals(reqContext.SqlId, StringComparison.OrdinalIgnoreCase))
                )
            {
                return false;
            }

            if (_filterOptions.IgnoreFullSqlIds != null 
                && _filterOptions.IgnoreFullSqlIds.Any()
                && _filterOptions.IgnoreFullSqlIds.Any(fullSqlId =>
                    fullSqlId.Equals(reqContext.FullSqlId, StringComparison.OrdinalIgnoreCase))
                )
            {
                return false;
            }

            #endregion
            
            if ((_filterOptions.StatementType & reqContext.Statement.StatementType)
                == StatementType.Unknown)
            {
                return false;
            }

            if (_filterOptions.Scopes != null 
                && _filterOptions.Scopes.Any()
                && !_filterOptions.Scopes.Any(scope =>
                    scope.Equals(reqContext.Scope, StringComparison.OrdinalIgnoreCase))
                )
            {
                return false;
            }

            if (_filterOptions.SqlIds != null 
                && _filterOptions.SqlIds.Any()
                && !_filterOptions.SqlIds.Any(sqlId =>
                    sqlId.Equals(reqContext.SqlId, StringComparison.OrdinalIgnoreCase))
                )
            {
                return false;
            }

            if (_filterOptions.FullSqlIds != null 
                && _filterOptions.FullSqlIds.Any()
                && !_filterOptions.FullSqlIds.Any(fullSqlId =>
                    fullSqlId.Equals(reqContext.FullSqlId, StringComparison.OrdinalIgnoreCase))
                )
            {
                return false;
            }

            return true;
        }
    }
}