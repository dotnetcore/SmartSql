using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using SmartSql;
using SmartSql.DbSession;
using SmartSql.Exceptions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SmartSqlExtensions
    {
        public static SmartSqlBuilder GetSmartSql(this IServiceProvider sp, string alias = SmartSqlBuilder.DEFAULT_ALIAS)
        {
            return sp.GetServices<SmartSqlBuilder>().FirstOrDefault(m => m.Alias == alias);
        }
        public static SmartSqlBuilder EnsureSmartSql(this IServiceProvider sp, string alias = SmartSqlBuilder.DEFAULT_ALIAS)
        {
            var smartsqlBuilder = sp.GetSmartSql(alias);
            if (smartsqlBuilder == null)
            {
                throw new SmartSqlException($"Can not find SmartSql.Alias:{alias} instance.");
            }
            return smartsqlBuilder;
        }
        public static IDbSessionFactory GetSessionFactory(this IServiceProvider sp, string alias = SmartSqlBuilder.DEFAULT_ALIAS)
        {
            return sp.GetSmartSql(alias)?.DbSessionFactory;
        }
        public static ISqlMapper GetSqlMapper(this IServiceProvider sp, string alias = SmartSqlBuilder.DEFAULT_ALIAS)
        {
            return sp.GetSmartSql(alias)?.SqlMapper;
        }
        public static IDbSessionStore GetSessionStore(this IServiceProvider sp, string alias = SmartSqlBuilder.DEFAULT_ALIAS)
        {
            return sp.GetSmartSql(alias)?.SmartSqlConfig.SessionStore;
        }
        public static ITransaction GetTransaction(this IServiceProvider sp, string alias = SmartSqlBuilder.DEFAULT_ALIAS)
        {
            return sp.GetSmartSql(alias)?.SqlMapper;
        }
    }
}
