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
        private static void AppendColumnName(StringBuilder sqlBuilder, DbProvider dbProvider, string paramName)
        {
            sqlBuilder.AppendFormat("{0}{1}{2}", dbProvider.ParameterNamePrefix, paramName,
                dbProvider.ParameterNameSuffix);
        }

        private static void AppendParameterName(StringBuilder sqlBuilder, DbProvider dbProvider, string paramName)
        {
            sqlBuilder.AppendFormat("{0}{1}", dbProvider.ParameterPrefix, paramName);
        }

        private static void AppendColumnEqParameter(StringBuilder sqlBuilder, DbProvider dbProvider,
            ColumnAttribute column)
        {
            sqlBuilder.AppendFormat("{0}{1}{2}={3}{4}", dbProvider.ParameterNamePrefix, column.Name,
                dbProvider.ParameterNameSuffix, dbProvider.ParameterPrefix, column.Property.Name);
        }

        private static String WrapColumnEqParameter(DbProvider dbProvider, ColumnAttribute col)
        {
            return
                $"{dbProvider.ParameterNamePrefix}{col.Name}{dbProvider.ParameterNameSuffix}={dbProvider.ParameterPrefix}{col.Property.Name}";
        }

        public static TEntity GetById<TEntity, TPrimaryKey>(this IDbSession dbSession, TPrimaryKey id)
        {
            return GetById<TEntity, TPrimaryKey>(dbSession, id, false);
        }

        public static TEntity GetById<TEntity, TPrimaryKey>(this IDbSession dbSession, TPrimaryKey id,
            bool enablePropertyChangedTrack)
        {
            var tableName = EntityMetaDataCache<TEntity>.TableName;
            var pkCol = EntityMetaDataCache<TEntity>.PrimaryKey;
            var idParam = new SqlParameter(pkCol.Property.Name, id, pkCol.Property.PropertyType)
            {
                TypeHandler = TypeHandlerCache<TPrimaryKey, TPrimaryKey>.Handler
            };
            var dbProvider = dbSession.SmartSqlConfig.Database.DbProvider;
            var sql = $"Select * From {tableName} Where {WrapColumnEqParameter(dbProvider, pkCol)};";
            return dbSession.QuerySingle<TEntity>(new RequestContext
            {
                EnablePropertyChangedTrack = enablePropertyChangedTrack,
                RealSql = sql,
                Request = new SqlParameterCollection {idParam}
            });
        }

        private static SqlParameterCollection ToSqlParameters<TEntity>(TEntity entity, bool ignoreCase)
        {
            return ignoreCase
                ? RequestConvertCache<TEntity, IgnoreCaseType>.Convert(entity)
                : RequestConvertCache<TEntity>.Convert(entity);
        }

        public static int Insert<TEntity>(this IDbSession dbSession, TEntity entity)
        {
            var dyParams = ToSqlParameters<TEntity>(entity, dbSession.SmartSqlConfig.Settings.IgnoreParameterCase);
            StringBuilder sqlBuilder = BuildInsertSql<TEntity>(dbSession.SmartSqlConfig.Database.DbProvider, dyParams);
            return dbSession.Execute(new RequestContext
            {
                RealSql = sqlBuilder.ToString(),
                Request = dyParams
            });
        }

        private static readonly ISetAccessorFactory _setAccessorFactory = EmitSetAccessorFactory.Instance;

        public static TPrimaryKey Insert<TEntity, TPrimaryKey>(this IDbSession dbSession, TEntity entity)
        {
            var dyParams = ToSqlParameters<TEntity>(entity, dbSession.SmartSqlConfig.Settings.IgnoreParameterCase);
            StringBuilder sqlBuilder = BuildInsertSql<TEntity>(dbSession.SmartSqlConfig.Database.DbProvider, dyParams);
            var dbProvider = dbSession.SmartSqlConfig.Database.DbProvider;
            var pkCol = EntityMetaDataCache<TEntity>.PrimaryKey;
            if (dbProvider.Type == DbProviderManager.POSTGRESQL_DBPROVIDER.Type)
            {
                sqlBuilder.AppendFormat(" Returning {0};", pkCol.Name);
            }
            else
            {
                sqlBuilder.Append(dbProvider.SelectAutoIncrement);
            }

            var id = dbSession.ExecuteScalar<TPrimaryKey>(new RequestContext
            {
                RealSql = sqlBuilder.ToString(),
                Request = dyParams
            });

            _setAccessorFactory.Create(pkCol.Property)(entity, id);
            return id;
        }

        private static StringBuilder BuildInsertSql<TEntity>(DbProvider dbProvider, SqlParameterCollection dyParams)
        {
            var tableName = EntityMetaDataCache<TEntity>.TableName;
            var isFirst = true;
            var columnBuilder = new StringBuilder();
            var paramBuilder = new StringBuilder();

            foreach (var paramKV in dyParams)
            {
                if (!EntityMetaDataCache<TEntity>.TryGetColumnByPropertyName(paramKV.Key, out var column))
                {
                    continue;
                }

                if (column.IsAutoIncrement)
                {
                    continue;
                }

                if (!isFirst)
                {
                    columnBuilder.Append(",");
                    paramBuilder.Append(",");
                }

                isFirst = false;
                AppendColumnName(columnBuilder, dbProvider, column.Name);
                AppendParameterName(paramBuilder, dbProvider, paramKV.Key);
            }

            var sqlBuilder = new StringBuilder();
            sqlBuilder.AppendFormat("Insert Into {0} ({1}) Values ({2})", tableName, columnBuilder.ToString(),
                paramBuilder.ToString());
            return sqlBuilder;
        }

        #region Delete

        public static int DeleteById<TEntity, TPrimaryKey>(this IDbSession dbSession, TPrimaryKey id)
        {
            var tableName = EntityMetaDataCache<TEntity>.TableName;
            var pkCol = EntityMetaDataCache<TEntity>.PrimaryKey;
            var idParam = new SqlParameter(pkCol.Property.Name, id, pkCol.Property.PropertyType)
            {
                TypeHandler = TypeHandlerCache<TPrimaryKey, TPrimaryKey>.Handler
            };
            var sql =
                $"Delete From {tableName} Where {WrapColumnEqParameter(dbSession.SmartSqlConfig.Database.DbProvider, pkCol)};";
            return dbSession.Execute(new RequestContext
            {
                RealSql = sql,
                Request = new SqlParameterCollection {idParam}
            });
        }

        public static int DeleteMany<TEntity, TPrimaryKey>(this IDbSession dbSession, IEnumerable<TPrimaryKey> ids)
        {
            var tableName = EntityMetaDataCache<TEntity>.TableName;
            var pkCol = EntityMetaDataCache<TEntity>.PrimaryKey;
            var sqlBuilder = new StringBuilder();
            sqlBuilder.AppendFormat("Delete From {0} Where {1} In (", tableName, pkCol.Name);
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
                if (index > 0)
                {
                    sqlBuilder.Append(",");
                }

                AppendParameterName(sqlBuilder, dbSession.SmartSqlConfig.Database.DbProvider, idName);
                index++;
            }

            sqlBuilder.Append(")");
            return dbSession.Execute(new RequestContext
            {
                RealSql = sqlBuilder.ToString(),
                Request = sqlParameters
            });
        }

        public static int DeleteAll<TEntity>(this IDbSession dbSession)
        {
            var tableName = EntityMetaDataCache<TEntity>.TableName;
            var sql = $"Delete From {tableName};";
            return dbSession.Execute(new RequestContext
            {
                RealSql = sql
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
            var entityProxy = entity as IEntityPropertyChangedTrackProxy;
            if (!enablePropertyChangedTrack.HasValue)
            {
                enablePropertyChangedTrack = entityProxy != null;
            }

            if (enablePropertyChangedTrack == true)
            {
                enablePropertyChangedTrack = entityProxy != null;
            }

            var dyParams = SqlParameterCollection.Create(entity, false);
            var tableName = EntityMetaDataCache<TEntity>.TableName;
            var pkCol = EntityMetaDataCache<TEntity>.PrimaryKey;
            var sqlBuilder = new StringBuilder();
            sqlBuilder.AppendFormat("Update {0} Set ", tableName);
            var isFirst = true;
            foreach (var paramKV in dyParams)
            {
                if (!EntityMetaDataCache<TEntity>.TryGetColumnByPropertyName(paramKV.Key, out var column))
                {
                    continue;
                }

                if (column.IsPrimaryKey)
                {
                    continue;
                }

                if (enablePropertyChangedTrack.Value && entityProxy.GetPropertyVersion(column.Property.Name) == 0)
                {
                    continue;
                }

                if (!isFirst)
                {
                    sqlBuilder.Append(",");
                }

                isFirst = false;
                AppendColumnEqParameter(sqlBuilder, dbSession.SmartSqlConfig.Database.DbProvider, column);
            }

            sqlBuilder.Append(" Where ");
            AppendColumnEqParameter(sqlBuilder, dbSession.SmartSqlConfig.Database.DbProvider, pkCol);
            return dbSession.Execute(new RequestContext
            {
                RealSql = sqlBuilder.ToString(),
                Request = dyParams
            });
        }

        #endregion
    }
}