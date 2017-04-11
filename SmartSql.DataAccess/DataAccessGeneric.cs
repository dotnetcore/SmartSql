using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Abstractions;
using SmartSql.DataAccess.Abstractions;
namespace SmartSql.DataAccess
{
    public class DataAccessGeneric<TEntity> : DataAccess, IReadDataAccess<TEntity>, IWriteDataAccess<TEntity>
        where TEntity : class
    {
        public DataAccessGeneric(String smartSqlMapConfigPath = "SmartSqlMapConfig.config") : base(smartSqlMapConfigPath)
        {

        }
        protected bool IsAutoPrimary { get; set; } = true;
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

        public IEnumerable<TResponse> GetListByPage<TResponse>(object paramObj, DataSourceChoice sourceChoice = DataSourceChoice.Read)
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
            if (IsAutoPrimary)
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
            return SqlMapper.Execute(new RequestContext
            {
                Scope = this.Scope,
                SqlId = DefaultSqlId.Delete,
                Request = new { Id = Id }
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
