using SmartSql.Abstractions;
using SmartSql.Abstractions.DbSession;
using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using System.Threading.Tasks;
using SmartSql.Abstractions.Logging;

namespace SmartSql
{
    public class SqlRuner
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(SqlBuilder));
        public ISqlBuilder SqlBuilder { get; }
        public ISmartSqlMapper SmartSqlMapper { get; }
        public SqlRuner(ISqlBuilder sqlBuilder, ISmartSqlMapper smartSqlMapper)
        {
            SqlBuilder = sqlBuilder;
            SmartSqlMapper = smartSqlMapper;
        }

        public T Run<T>(IRequestContext context, DataSourceChoice sourceChoice, Func<String, IDbConnectionSession, T> runSql)
        {
            IDbConnectionSession session = SmartSqlMapper.SessionStore.LocalSession;
            
            if (session == null)
            {
                session = SmartSqlMapper.CreateDbSession(sourceChoice);
            }

            string sqlStr = SqlBuilder.BuildSql(context);
            try
            {
                return runSql(sqlStr, session);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (!session.IsTransactionOpen)
                {
                    session.CloseConnection();
                }
            }
        }

        public async Task<T> RunAsync<T>(IRequestContext context, IDbConnectionSession session, DataSourceChoice sourceChoice, Func<String, IDbConnectionSession, Task<T>> runSql)
        {
            if (session == null)
            {
                session = SmartSqlMapper.CreateDbSession(sourceChoice);
            }

            string sqlStr = SqlBuilder.BuildSql(context);
            try
            {
                return await runSql(sqlStr, session);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //if (!session.IsTransactionOpen)
                //{
                //    session.CloseConnection();
                //}
            }
        }

    }

}

