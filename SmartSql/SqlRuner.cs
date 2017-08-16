using SmartSql.Abstractions;
using SmartSql.Abstractions.DbSession;
using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using System.Threading.Tasks;
using SmartSql.Abstractions.Cache;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace SmartSql
{
    public class SqlRuner
    {
        private readonly ILogger _logger;
        public ISqlBuilder SqlBuilder { get; }
        public ISmartSqlMapper SmartSqlMapper { get; }
        public SqlRuner(ILoggerFactory loggerFactory, ISqlBuilder sqlBuilder, ISmartSqlMapper smartSqlMapper)
        {
            _logger = loggerFactory.CreateLogger<SqlRuner>();
            SqlBuilder = sqlBuilder;
            SmartSqlMapper = smartSqlMapper;
        }

        public T Run<T>(RequestContext context, DataSourceChoice sourceChoice, Func<String, IDbConnectionSession, T> runSql)
        {

            IDbConnectionSession session = SmartSqlMapper.SessionStore.LocalSession;

            if (session == null)
            {
                session = SmartSqlMapper.CreateDbSession(sourceChoice);
            }

            string sqlStr = SqlBuilder.BuildSql(context);
            try
            {
                T result = runSql(sqlStr, session);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (session.LifeCycle == DbSessionLifeCycle.Transient)
                {
                    session.CloseConnection();
                }
            }
        }

        public async Task<T> RunAsync<T>(RequestContext context, DataSourceChoice sourceChoice, Func<String, IDbConnectionSession, Task<T>> runSql)
        {

            IDbConnectionSession session = SmartSqlMapper.SessionStore.LocalSession;
            if (session == null)
            {
                session = SmartSqlMapper.CreateDbSession(sourceChoice);
            }

            string sqlStr = SqlBuilder.BuildSql(context);
            try
            {
                T result = await runSql(sqlStr, session);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (session.LifeCycle == DbSessionLifeCycle.Transient)
                {
                    session.CloseConnection();
                }
            }
        }

    }

}

