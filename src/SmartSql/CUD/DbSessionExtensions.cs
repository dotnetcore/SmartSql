using SmartSql.Annotations;
using SmartSql.Data;
using SmartSql.DbSession;
using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Reflection;
using SmartSql.Reflection.PropertyAccessor;
using SmartSql.DataSource;
using SmartSql.CUD;
using SmartSql.Reflection.Convert;
using SmartSql.Reflection.EntityProxy;
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
            var scope = EntityMetaDataCache<TEntity>.TableName;
            var pkCol = EntityMetaDataCache<TEntity>.PrimaryKey;
            var idParam = new SqlParameter(pkCol.Property.Name, id, pkCol.Property.PropertyType)
            {
                TypeHandler = TypeHandlerCache<TPrimaryKey, TPrimaryKey>.Handler
            };
            return dbSession.QuerySingle<TEntity>(new RequestContext
            {
                EnablePropertyChangedTrack = enablePropertyChangedTrack,
                Scope = scope,
                SqlId = CUDStatementName.GetById,
                Request = new SqlParameterCollection {idParam}
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
            var scope = EntityMetaDataCache<TEntity>.TableName;
            return dbSession.Execute(new RequestContext
            {
                Scope = scope,
                SqlId = CUDStatementName.Insert,
                Request = dyParams
            });
        }

        public static TPrimaryKey Insert<TEntity, TPrimaryKey>(this IDbSession dbSession, TEntity entity)
        {
            var dyParams = ToSqlParameters<TEntity>(entity, dbSession.SmartSqlConfig.Settings.IgnoreParameterCase);
            var scope = EntityMetaDataCache<TEntity>.TableName;

            var pkCol = EntityMetaDataCache<TEntity>.PrimaryKey;


            var id = dbSession.ExecuteScalar<TPrimaryKey>(new RequestContext
            {
                Scope = scope,
                SqlId = CUDStatementName.InsertReturnId,
                Request = dyParams
            });

            _setAccessorFactory.Create(pkCol.Property)(entity, id);
            return id;
        }

        #endregion


        #region Delete

        public static int DeleteById<TEntity, TPrimaryKey>(this IDbSession dbSession, TPrimaryKey id)
        {
            var scope = EntityMetaDataCache<TEntity>.TableName;
            var pkCol = EntityMetaDataCache<TEntity>.PrimaryKey;
            var idParam = new SqlParameter(pkCol.Property.Name, id, pkCol.Property.PropertyType)
            {
                TypeHandler = TypeHandlerCache<TPrimaryKey, TPrimaryKey>.Handler
            };
            //var sql =
            //    $"Delete From {tableName} Where {WrapColumnEqParameter(dbSession.SmartSqlConfig.Database.DbProvider, pkCol)}";
            return dbSession.Execute(new RequestContext
            {
                //RealSql = sql,
                Scope = scope,
                SqlId = CUDStatementName.DeleteById,
                Request = new SqlParameterCollection {idParam}
            });
        }

        public static int DeleteMany<TEntity, TPrimaryKey>(this IDbSession dbSession, IEnumerable<TPrimaryKey> ids)
        {
            var scope = EntityMetaDataCache<TEntity>.TableName;
            var pkCol = EntityMetaDataCache<TEntity>.PrimaryKey;
            //var sqlBuilder = new StringBuilder();
            //sqlBuilder.AppendFormat("Delete From {0} Where {1} In (", tableName, pkCol.Name);
            var sqlParameters = new SqlParameterCollection();
            var index = 0;
            foreach (var id in ids)
            {
                var idName = $"{pkCol.Name}_{index}";
                var sqlParameter = new SqlParameter(idName, id, id.GetType())
                {
                    TypeHandler = TypeHandlerCache<TPrimaryKey, TPrimaryKey>.Handler
                };
                sqlParameters.TryAdd(sqlParameter);
                //if (index > 0)
                //{
                //    sqlBuilder.Append(",");
                //}

                //AppendParameterName(sqlBuilder, dbSession.SmartSqlConfig.Database.DbProvider, idName);
                index++;
            }

            //sqlBuilder.Append(")");
            return dbSession.Execute(new RequestContext
            {
                //RealSql = sqlBuilder.ToString(),
                Scope = scope,
                SqlId=CUDStatementName.DeleteMany,
                Request = sqlParameters
            });
        }

        public static int DeleteAll<TEntity>(this IDbSession dbSession)
        {
            var scope = EntityMetaDataCache<TEntity>.TableName;
            //var sql = $"Delete From {tableName}";
            return dbSession.Execute(new RequestContext
            {
                //RealSql = sql
                SqlId=CUDStatementName.DeleteAll,
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
            //var entityProxy = entity as IEntityPropertyChangedTrackProxy;
            //if (!enablePropertyChangedTrack.HasValue)
            //{
            //    enablePropertyChangedTrack = entityProxy != null;
            //}

            //if (enablePropertyChangedTrack == true)
            //{
            //    enablePropertyChangedTrack = entityProxy != null;
            //}

            var dyParams = SqlParameterCollection.Create(entity, false);
            var scope = EntityMetaDataCache<TEntity>.TableName;
            //var pkCol = EntityMetaDataCache<TEntity>.PrimaryKey;
            //var sqlBuilder = new StringBuilder();
            //sqlBuilder.AppendFormat("Update {0} Set ", tableName);
            //var isFirst = true;
            //foreach (var paramKV in dyParams)
            //{
            //    if (!EntityMetaDataCache<TEntity>.TryGetColumnByPropertyName(paramKV.Key, out var column))
            //    {
            //        continue;
            //    }

            //    if (column.IsPrimaryKey)
            //    {
            //        continue;
            //    }

            //    if (enablePropertyChangedTrack.Value && entityProxy.GetPropertyVersion(column.Property.Name) == 0)
            //    {
            //        continue;
            //    }

            //    if (!isFirst)
            //    {
            //        sqlBuilder.Append(",");
            //    }

            //    isFirst = false;
            //    AppendColumnEqParameter(sqlBuilder, dbSession.SmartSqlConfig.Database.DbProvider, column);
            //}

            //sqlBuilder.Append(" Where ");
            //AppendColumnEqParameter(sqlBuilder, dbSession.SmartSqlConfig.Database.DbProvider, pkCol);

            return dbSession.Execute(new RequestContext
            {
                Scope = scope,
                SqlId=CUDStatementName.Update,
                //RealSql = sqlBuilder.ToString(),
                Request = dyParams
            });
        }

        #endregion
    }
}