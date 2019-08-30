using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SmartSql.DbSession;

namespace SmartSql
{
    public static class ISqlMapperExtensions
    {
        internal static TResult ExecuteImpl<TResult>(this ISqlMapper sqlMapper, Func<IDbSession, TResult> executeFunc)
        {
            //Session 释放原则：谁开启，谁释放
            var dbSession = sqlMapper.SessionStore.LocalSession;
            var ownSession = dbSession == null;
            try
            {
                if (ownSession)
                {
                    dbSession = sqlMapper.SessionStore.Open();
                }

                return executeFunc(dbSession);
            }
            finally
            {
                if (ownSession)
                {
                    sqlMapper.SessionStore.Dispose();
                }
            }
        }

        public static int DeleteAll<TEntity>(this ISqlMapper sqlMapper)
        {
            return ExecuteImpl(sqlMapper, session => session.DeleteAll<TEntity>());
        }

        public static int DeleteById<TEntity, TPrimaryKey>(this ISqlMapper sqlMapper, TPrimaryKey id)
        {
            return ExecuteImpl(sqlMapper, session => session.DeleteById<TEntity, TPrimaryKey>(id));
        }

        public static int DeleteMany<TEntity, TPrimaryKey>(this ISqlMapper sqlMapper, IEnumerable<TPrimaryKey> ids)
        {
            return ExecuteImpl(sqlMapper, session => session.DeleteMany<TEntity, TPrimaryKey>(ids));
        }

        public static TEntity GetById<TEntity, TPrimaryKey>(this ISqlMapper sqlMapper, TPrimaryKey id)
        {
            return ExecuteImpl(sqlMapper, session => session.GetById<TEntity, TPrimaryKey>(id));
        }

        public static TEntity GetById<TEntity, TPrimaryKey>(this ISqlMapper sqlMapper, TPrimaryKey id,
            bool enablePropertyChangedTrack)
        {
            return ExecuteImpl(sqlMapper,
                session => session.GetById<TEntity, TPrimaryKey>(id, enablePropertyChangedTrack));
        }

        public static TPrimaryKey Insert<TEntity, TPrimaryKey>(this ISqlMapper sqlMapper, TEntity entity)
        {
            return ExecuteImpl(sqlMapper, session => session.Insert<TEntity, TPrimaryKey>(entity));
        }

        public static int Insert<TEntity>(this ISqlMapper sqlMapper, TEntity entity)
        {
            return ExecuteImpl(sqlMapper, session => session.Insert(entity));
        }

        public static int Update<TEntity>(this ISqlMapper sqlMapper, TEntity entity)
        {
            return ExecuteImpl(sqlMapper, session => session.Update(entity));
        }

        public static int Update<TEntity>(this ISqlMapper sqlMapper, TEntity entity, bool enablePropertyChangedTrack)
        {
            return ExecuteImpl(sqlMapper, session => session.Update(entity, enablePropertyChangedTrack));
        }

        public static int DyUpdate<TEntity>(this ISqlMapper sqlMapper, object entity)
        {
            return ExecuteImpl(sqlMapper, session => session.DyUpdate<TEntity>(entity));
        }

        public static int DyUpdate<TEntity>(this ISqlMapper sqlMapper, object entity, bool? enablePropertyChangedTrack)
        {
            return ExecuteImpl(sqlMapper, session => session.DyUpdate<TEntity>(entity, enablePropertyChangedTrack));
        }
        
        
        
        public static IList<dynamic> QueryDynamic(this ISqlMapper sqlMapper,  AbstractRequestContext requestContext)
        {
            return ExecuteImpl(sqlMapper, session => session.QueryDynamic(requestContext));
        }

        public static IList<IDictionary<String, object>> QueryDictionary(this ISqlMapper sqlMapper, 
            AbstractRequestContext requestContext)
        {
            return ExecuteImpl(sqlMapper, session => session.QueryDictionary(requestContext));
        }

        public static dynamic QuerySingleDynamic(this ISqlMapper sqlMapper,  AbstractRequestContext requestContext)
        {
            return ExecuteImpl(sqlMapper, session => session.QuerySingleDynamic(requestContext));
        }

        public static IDictionary<String, object> QuerySingleDictionary(this ISqlMapper sqlMapper, 
            AbstractRequestContext requestContext)
        {
            return ExecuteImpl(sqlMapper, session => session.QuerySingleDictionary(requestContext));
        }


    }
}