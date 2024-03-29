﻿using SmartSql.Data;
using SmartSql.DbSession;
using System.Collections.Generic;
using SmartSql.Reflection;
using SmartSql.Reflection.PropertyAccessor;
using SmartSql.CUD;
using SmartSql.Reflection.Convert;
using SmartSql.TypeHandlers;

namespace SmartSql
{
    public static partial class DbSessionExtensions
    {
        private static readonly ISetAccessorFactory _setAccessorFactory = EmitSetAccessorFactory.Instance;


        #region GetById

        public static TEntity GetById<TEntity, TPrimaryKey>(this IDbSession dbSession, TPrimaryKey id)
        {
            return GetById<TEntity, TPrimaryKey>(dbSession, id, false);
        }

        public static TEntity GetById<TEntity, TPrimaryKey>(this IDbSession dbSession, TPrimaryKey id,
            bool enablePropertyChangedTrack)
        {
            var scope = EntityMetaDataCache<TEntity>.Scope;
            var pkCol = EntityMetaDataCache<TEntity>.PrimaryKey;
            var idParam = new SqlParameter(pkCol.Property.Name, id, pkCol.Property.PropertyType)
            {
                TypeHandler = TypeHandlerCache<TPrimaryKey, TPrimaryKey>.Handler
            };
            return dbSession.QuerySingle<TEntity>(new RequestContext
            {
                EnablePropertyChangedTrack = enablePropertyChangedTrack,
                Scope = scope,
                SqlId = CUDStatementName.GET_BY_ID,
                Request = new SqlParameterCollection { idParam }
            });
        }

        #endregion


        #region Insert

        private static SqlParameterCollection ToSqlParameters<TEntity>(TEntity entity, bool ignoreCase)
        {
            return ignoreCase
                ? RequestConvertCache<TEntity, IgnoreCaseType>.Convert(entity)
                : RequestConvertCache<TEntity>.Convert(entity);
        }

        public static int Insert<TEntity>(this IDbSession dbSession, TEntity entity)
        {
            var dyParams = ToSqlParameters<TEntity>(entity, dbSession.SmartSqlConfig.Settings.IgnoreParameterCase);
            var scope = EntityMetaDataCache<TEntity>.Scope;
            return dbSession.Execute(new RequestContext
            {
                Scope = scope,
                SqlId = CUDStatementName.INSERT,
                Request = dyParams
            });
        }

        public static TPrimaryKey Insert<TEntity, TPrimaryKey>(this IDbSession dbSession, TEntity entity)
        {
            var dyParams = ToSqlParameters<TEntity>(entity, dbSession.SmartSqlConfig.Settings.IgnoreParameterCase);
            var scope = EntityMetaDataCache<TEntity>.Scope;

            var pkCol = EntityMetaDataCache<TEntity>.PrimaryKey;


            var id = dbSession.ExecuteScalar<TPrimaryKey>(new RequestContext
            {
                Scope = scope,
                SqlId = CUDStatementName.INSERT_RETURN_ID,
                Request = dyParams
            });

            _setAccessorFactory.Create(pkCol.Property)(entity, id);
            return id;
        }

        #endregion


        #region Delete

        public static int DeleteById<TEntity, TPrimaryKey>(this IDbSession dbSession, TPrimaryKey id)
        {
            var scope = EntityMetaDataCache<TEntity>.Scope;
            var pkCol = EntityMetaDataCache<TEntity>.PrimaryKey;
            var idParam = new SqlParameter(pkCol.Property.Name, id, pkCol.Property.PropertyType)
            {
                TypeHandler = TypeHandlerCache<TPrimaryKey, TPrimaryKey>.Handler
            };
            return dbSession.Execute(new RequestContext
            {
                Scope = scope,
                SqlId = CUDStatementName.DELETE_BY_ID,
                Request = new SqlParameterCollection { idParam }
            });
        }

        public static int DeleteMany<TEntity, TPrimaryKey>(this IDbSession dbSession, IEnumerable<TPrimaryKey> ids)
        {
            var scope = EntityMetaDataCache<TEntity>.Scope;
            var pkCol = EntityMetaDataCache<TEntity>.PrimaryKey;
            var sqlParameters = new SqlParameterCollection();

            sqlParameters.TryAdd($"{pkCol.Name}s", ids);

            return dbSession.Execute(new RequestContext
            {
                Scope = scope,
                SqlId = CUDStatementName.DELETE_MANY,
                Request = sqlParameters
            });
        }

        public static int DeleteAll<TEntity>(this IDbSession dbSession)
        {
            var scope = EntityMetaDataCache<TEntity>.Scope;
            return dbSession.Execute(new RequestContext
            {
                SqlId = CUDStatementName.DELETE_ALL,
                Scope = scope
            });
        }

        #endregion

        #region Update

        public static int Update<TEntity>(this IDbSession dbSession, TEntity entity)
        {
            return DyUpdate<TEntity>(dbSession, entity, null);
        }

        public static int Update<TEntity>(this IDbSession dbSession, TEntity entity, bool enablePropertyChangedTrack)
        {
            return DyUpdate<TEntity>(dbSession, entity, enablePropertyChangedTrack);
        }

        public static int DyUpdate<TEntity>(this IDbSession dbSession, object entity)
        {
            return DyUpdate<TEntity>(dbSession, entity, null);
        }

        public static int DyUpdate<TEntity>(this IDbSession dbSession, object entity, bool? enablePropertyChangedTrack)
        {
            var dyParams = SqlParameterCollection.Create(entity, false);
            var scope = EntityMetaDataCache<TEntity>.Scope;

            return dbSession.Execute(new RequestContext
            {
                Scope = scope,
                SqlId = CUDStatementName.UPDATE,
                Request = dyParams
            });
        }

        #endregion
    }
}