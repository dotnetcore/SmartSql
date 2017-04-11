using SmartSql.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.DataAccess.Abstractions;
using System.Data;
using SmartSql.Abstractions.DbSession;

namespace SmartSql.DataAccess
{
    public abstract class DataAccess : ITransaction
    {
        public DataAccess(String smartSqlMapConfigPath = "SmartSqlMapConfig.config")
        {
            SmartSqlMapConfigPath = smartSqlMapConfigPath;
            InitScope();
        }

        public String SmartSqlMapConfigPath { get; private set; }

        public string Scope { get; protected set; }

        protected abstract void InitScope();

        public ISmartSqlMapper SqlMapper { get { return MapperContainer.GetSqlMapper(SmartSqlMapConfigPath); } }

        #region Transaction
        /// <summary>
        /// 开启事务
        /// </summary>
        /// <returns></returns>
        public virtual IDbConnectionSession BeginTransaction()
        {
            return SqlMapper.BeginTransaction();
        }

        public IDbConnectionSession BeginTransaction(IsolationLevel isolationLevel)
        {
            return SqlMapper.BeginTransaction(isolationLevel);
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public virtual void CommitTransaction()
        {
            SqlMapper.CommitTransaction();
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public virtual void RollbackTransaction()
        {
            SqlMapper.RollbackTransaction();
        }


        #endregion
    }
}
