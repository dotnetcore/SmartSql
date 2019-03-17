using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql
{
    public static class ISqlMapperExtensions
    {
        public static int DeleteAll<TEntity>(this ISqlMapper sqlMapper)
        {
            return sqlMapper.SessionStore.Open().DeleteAll<TEntity>();
        }
        public static int DeleteById<TEntity>(this ISqlMapper sqlMapper, object id)
        {
            return sqlMapper.SessionStore.Open().DeleteById<TEntity>(id);
        }
        public static int DeleteMany<TEntity, TPrimaryKey>(this ISqlMapper sqlMapper, IEnumerable<TPrimaryKey> ids)
        {
            return sqlMapper.SessionStore.Open().DeleteMany<TEntity, TPrimaryKey>(ids);
        }
        public static int DyUpdate<TEntity>(this ISqlMapper sqlMapper, object entity)
        {
            return sqlMapper.SessionStore.Open().DyUpdate<TEntity>(entity);
        }
        public static TEntity GetById<TEntity>(this ISqlMapper sqlMapper, object id)
        {
            return sqlMapper.SessionStore.Open().GetById<TEntity>(id);
        }
        public static TPrimaryKey Insert<TEntity, TPrimaryKey>(this ISqlMapper sqlMapper, TEntity entity)
        {
            return sqlMapper.SessionStore.Open().Insert<TEntity, TPrimaryKey>(entity);
        }
        public static int Insert<TEntity>(this ISqlMapper sqlMapper, TEntity entity)
        {
            return sqlMapper.SessionStore.Open().Insert<TEntity>(entity);
        }
        public static int Update<TEntity>(this ISqlMapper sqlMapper, TEntity entity)
        {
            return sqlMapper.SessionStore.Open().Update<TEntity>(entity);
        }

    }
}
