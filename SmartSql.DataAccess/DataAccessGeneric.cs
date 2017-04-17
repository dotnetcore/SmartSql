using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Abstractions;
using SmartSql.DataAccess.Abstractions;
namespace SmartSql.DataAccess
{

    /// <summary>
    /// 泛型数据库访问对象
    /// 提供常规方法
    /// </summary>
    /// <typeparam name="TEntity">Table Entity</typeparam>
    public class DataAccessGeneric<TEntity> : DataAccess, IReadDataAccess<TEntity>, IWriteDataAccess<TEntity>
        where TEntity : class
    {
        public DataAccessGeneric(String smartSqlMapConfigPath = "SmartSqlMapConfig.config") : base(smartSqlMapConfigPath)
        {

        }

        protected override void InitScope()
        {
            Scope = typeof(TEntity).Name;
        }
        #region Read
        public TEntity GetEntity(object paramObj, DataSourceChoice sourceChoice = DataSourceChoice.Read)
        {
            return SqlMapper.QuerySingle<TEntity>(new RequestContext
            {
                Scope = this.Scope,
                SqlId = DefaultSqlId.GetEntity,
                Request = paramObj
            }, sourceChoice);
        }

        public TEntity GetEntity<TPrimary>(TPrimary Id, DataSourceChoice sourceChoice = DataSourceChoice.Read)
        {
            return GetEntity(new { Id = Id }, sourceChoice);
        }

        public IEnumerable<TResponse> GetList<TResponse>(object paramObj, DataSourceChoice sourceChoice = DataSourceChoice.Read)
        {
            return SqlMapper.Query<TResponse>(new RequestContext
            {
                Scope = this.Scope,
                SqlId = DefaultSqlId.GetList,
                Request = paramObj
            }, sourceChoice);
        }

        public IEnumerable<TResponse> GetListByPage<TResponse>(object paramObj,  DataSourceChoice sourceChoice = DataSourceChoice.Read)
        {
            return SqlMapper.Query<TResponse>(new RequestContext
            {
                Scope = this.Scope,
                SqlId = DefaultSqlId.GetListByPage,
                Request = paramObj
            }, sourceChoice);
        }

        public int GetRecord(object paramObj, DataSourceChoice sourceChoice = DataSourceChoice.Read)
        {
            return SqlMapper.QuerySingle<int>(new RequestContext
            {
                Scope = this.Scope,
                SqlId = DefaultSqlId.GetRecord,
                Request = paramObj
            }, sourceChoice);
        }

        public bool IsExist(object paramObj, DataSourceChoice sourceChoice = DataSourceChoice.Read)
        {
            return SqlMapper.QuerySingle<int>(new RequestContext
            {
                Scope = this.Scope,
                SqlId = DefaultSqlId.GetRecord,
                Request = paramObj
            }, sourceChoice) > 0;
        }

        #endregion
        #region Write
        public TPrimary Insert<TPrimary>(TEntity entity)
        {
            bool isNoneIdentity = typeof(TPrimary) == typeof(NoneIdentity);
            if (!isNoneIdentity)
            {
                return SqlMapper.ExecuteScalar<TPrimary>(new RequestContext
                {
                    Scope = this.Scope,
                    SqlId = DefaultSqlId.Insert,
                    Request = entity
                });
            }
            else
            {
                SqlMapper.Execute(new RequestContext
                {
                    Scope = this.Scope,
                    SqlId = DefaultSqlId.Insert,
                    Request = entity
                });
                return default(TPrimary);
            }
        }
        public int Delete<TPrimary>(TPrimary Id)
        {
            return Delete(new { Id = Id });
        }

        public int Delete(object paramObj)
        {
            return SqlMapper.Execute(new RequestContext
            {
                Scope = this.Scope,
                SqlId = DefaultSqlId.Delete,
                Request = paramObj
            });
        }

        public int Update(TEntity entity)
        {
            return SqlMapper.Execute(new RequestContext
            {
                Scope = this.Scope,
                SqlId = DefaultSqlId.Update,
                Request = entity
            });
        }
        #endregion
    }

}
