using SmartSql.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.DataAccess.Abstractions;
using System.Data;
using SmartSql.Abstractions.DbSession;
using Microsoft.Extensions.Logging;

namespace SmartSql.DataAccess
{
    public abstract class DataAccess : Abstractions.ITransaction
    {
        public DataAccess(String smartSqlMapConfigPath = "SmartSqlMapConfig.xml")
        {
            SqlMapper = MapperContainer.Instance.GetSqlMapper(smartSqlMapConfigPath);
            InitScope();
        }
        public DataAccess(ILoggerFactory loggerFactory, String smartSqlMapConfigPath = "SmartSqlMapConfig.xml")
        {
            SqlMapper = MapperContainer.Instance.GetSqlMapper(new SmartSqlOptions
            {
                ConfigPath = smartSqlMapConfigPath,
                LoggerFactory = loggerFactory
            });
            InitScope();
        }
        public DataAccess(ISmartSqlMapper sqlMapper)
        {
            SqlMapper = sqlMapper;
            InitScope();
        }
        public string Scope { get; protected set; }

        protected abstract void InitScope();

        public ISmartSqlMapper SqlMapper { get; }

        #region Transaction
        /// <summary>
        /// 开启事务
        /// </summary>
        /// <returns></returns>
        public virtual IDbConnectionSession BeginTransaction()
        {
            return SqlMapper.BeginTransaction();
        }

        public virtual IDbConnectionSession BeginTransaction(IsolationLevel isolationLevel)
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
