using SmartSql.Annotations;
using SmartSql.Configuration;
using SmartSql.Configuration.Tags;
using SmartSql.DataSource;
using SmartSql.Reflection.TypeConstants;
using SmartSql.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SmartSql.CUD
{
    public class CUDSqlGenerator : ICUDSqlGenerator
    {

        public IReadOnlyDictionary<string, Statement> StatementList
        {
            get
            {
                if(statementList == null)
                {
                    throw new ArgumentNullException(nameof(statementList), "please call Init Method before access this Property");
                }
                return _statementListCache ?? (_statementListCache = new ReadOnlyDictionary<string, Statement>(statementList));
            }
        }

        private IReadOnlyDictionary<string, Statement> _statementListCache;
        private IDictionary<string, Func<Statement>> GeneratorFuncList;
        private DbProvider provider;
        private StatementAnalyzer analyzer;
        private SqlMap sqlMap;
        private string tableName;
        private ColumnAttribute pkCol;
        private Type entityMetaDataCache;
        private IDictionary<string, Statement> statementList;


        public CUDSqlGenerator()
        {
            statementList = new ConcurrentDictionary<string, Statement>();
            GeneratorFuncList = new Dictionary<string, Func<Statement>>
            {
                { CUDStatementName.Insert, BuildInsert },
                { CUDStatementName.Update, BuildUpdate },
                { CUDStatementName.DeleteById, BuildDeleteById },
                { CUDStatementName.DeleteAll, BuildDeleteAll },
                { CUDStatementName.DeleteMany, BuildDeleteMany },
            };
            
        }

        private Statement BuildStatement(string statementId, string sql)
        {
            return new Statement()
            {
                SqlMap = sqlMap,
                Id = statementId,
                CommandType = System.Data.CommandType.Text,
                StatementType = analyzer.Analyse(sql),
                SqlTags = new List<ITag>
                    {
                        new SqlText(sql, provider.ParameterPrefix)
                    },
            };
        }


        public Statement BuildDeleteAll()
        {
            var sql = $"delete from {tableName}";
            return BuildStatement(CUDStatementName.DeleteAll, sql);
        }

        public Statement BuildDeleteById()
        {
            var sql =
                $"Delete From {tableName} Where {WrapColumnEqParameter(provider, pkCol)}";

            return BuildStatement(CUDStatementName.DeleteById, sql);
        }

        public Statement BuildDeleteMany()
        {
            //  需要在执行前，替换#ids#为真实变量。此处的#ids#为点位符
            var sql = $"Delete From {tableName} Where {pkCol.Name} In (#ids#)";
            return BuildStatement(CUDStatementName.DeleteMany, sql);
        }

        public Statement BuildInsert()
        {
            var cols = GetEntityMetaData<SortedDictionary<int, ColumnAttribute>>("IndexColumnMaps");
            string colNames = string.Empty;
            string colVals = string.Empty;
            foreach (var col in cols)
            {
                colNames += $",{FormatColumnName(provider, col.Value.Name)}";
                colVals += $",{FormatParameterName(provider, col.Value.Name)}";
            }
            var sql = $"insert into {tableName} ({colNames.Substring(1)}) values ({colVals.Substring(1)})";
            return BuildStatement(CUDStatementName.Insert, sql);
        }

        public Statement BuildUpdate()
        {
            //  需要在执行前，替换#cols#为真实sql语句。此处的#cols#为点位符
            var sql = $"update {tableName} set #cols# where {WrapColumnEqParameter(provider, pkCol)}";
            return BuildStatement(CUDStatementName.Update, sql);
        }

        public void Init(SqlMap map, Type entityType)
        {
            var config = map.SmartSqlConfig;
            sqlMap = map;
            provider = config.Database.DbProvider;
            analyzer = new StatementAnalyzer();
            entityMetaDataCache = EntityMetaDataCacheType.MakeGenericType(entityType);
            tableName = GetEntityMetaData<string>("TableName");
            pkCol = GetEntityMetaData<ColumnAttribute>("PrimaryKey");

            if (GeneratorFuncList != null && GeneratorFuncList.Count > 0)
            {
                foreach (var item in GeneratorFuncList)
                {
                    statementList.Add(item.Key, item.Value());
                }
            }
        }

        private TData GetEntityMetaData<TData>(string propertyName)
        {
            return (TData)entityMetaDataCache.GetProperty(propertyName).GetValue(null);
        }

        private static string WrapColumnEqParameter(DbProvider dbProvider, ColumnAttribute col)
        {
            return
                $"{dbProvider.ParameterNamePrefix}{col.Name}{dbProvider.ParameterNameSuffix}={dbProvider.ParameterPrefix}{col.Property.Name}";
        }

        private static string FormatColumnName(DbProvider dbProvider, string paramName)
        {
            return $"{dbProvider.ParameterNamePrefix}{paramName}{dbProvider.ParameterNameSuffix}";
        }

        private static string FormatParameterName(DbProvider dbProvider, string paramName)
        {
            return $"{dbProvider.ParameterPrefix}{paramName}";
        }
    }
}
