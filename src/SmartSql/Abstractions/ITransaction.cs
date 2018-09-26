using SmartSql.Abstractions.DbSession;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.Abstractions
{
    public interface ITransaction
    {
        /// <summary>
        /// 开启事务
        /// </summary>
        /// <returns></returns>
        IDbConnectionSession BeginTransaction();
        /// <summary>
        /// 开启事务
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        IDbConnectionSession BeginTransaction(RequestContext context);
        /// <summary>
        /// 开启事务
        /// </summary>
        /// <param name="isolationLevel">事务级别</param>
        /// <returns></returns>
        IDbConnectionSession BeginTransaction(IsolationLevel isolationLevel);
        /// <summary>
        /// 开启事务
        /// </summary>
        /// <param name="context"></param>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        IDbConnectionSession BeginTransaction(RequestContext context, IsolationLevel isolationLevel);
        /// <summary>
        /// 提交事务
        /// </summary>
        void CommitTransaction();
        /// <summary>
        /// 回滚事务
        /// </summary>
        void RollbackTransaction();
    }
}
