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
                if(statementList == null || statementList.Count == 0)
                {
                    throw new ArgumentNullException(nameof(statementList), "please call Init Method before access this Property");
                }
                return _statementListCache ?? (_statementListCache = new ReadOnlyDictionary<string, Statement>(statementList));
            }
        }

        private IReadOnlyDictionary<string, Statement> _statementListCache;
        private IDictionary<string, Func<GeneratorParams, Statement>> GeneratorFuncList;
        private DbProvider provider;
        private StatementAnalyzer analyzer;
        private IDictionary<string, Statement> statementList;


        public CUDSqlGenerator(SmartSqlConfig config)
        {
            statementList = new ConcurrentDictionary<string, Statement>();
            GeneratorFuncList = new Dictionary<string, Func<GeneratorParams, Statement>>
            {
                { CUDStatementName.Insert, BuildInsert },
                { CUDStatementName.Update, BuildUpdate },
                { CUDStatementName.DeleteById, BuildDeleteById },
                { CUDStatementName.DeleteAll, BuildDeleteAll },
                { CUDStatementName.DeleteMany, BuildDeleteMany },
            };
            provider = config.Database.DbProvider;
            analyzer = new StatementAnalyzer();
        }

        private Statement BuildStatement(string statementId, string sql, SqlMap sqlMap)
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

        public Statement BuildDeleteAll(GeneratorParams gParams)
        {
            var sql = $"delete from {gParams.TableName}";
            return BuildStatement(CUDStatementName.DeleteAll, sql, gParams.Map);
        }

        public Statement BuildDeleteById(GeneratorParams gParams)
        {
            var sql =
                $"Delete From {gParams.TableName} Where {WrapColumnEqParameter(provider, gParams.PkCol)}";

            return BuildStatement(CUDStatementName.DeleteById, sql, gParams.Map);
        }

        public Statement BuildDeleteMany(GeneratorParams gParams)
        {
            //  需要在执行前，替换#ids#为真实变量。此处的#ids#为点位符
            var sql = $"Delete From {gParams.TableName} Where {gParams.PkCol.Name} In (#ids#)";
            return BuildStatement(CUDStatementName.DeleteMany, sql, gParams.Map);
        }

        public Statement BuildInsert(GeneratorParams gParams)
        {
            var cols = gParams.ColumnMaps;
            string colNames = string.Empty;
            string colVals = string.Empty;
            foreach (var col in cols)
            {
                colNames += $",{FormatColumnName(provider, col.Value.Name)}";
                colVals += $",{FormatParameterName(provider, col.Value.Name)}";
            }
            var sql = $"insert into {gParams.TableName} ({colNames.Substring(1)}) values ({colVals.Substring(1)})";
            return BuildStatement(CUDStatementName.Insert, sql, gParams.Map);
        }

        public Statement BuildUpdate(GeneratorParams gParams)
        {
            //  需要在执行前，替换#cols#为真实sql语句。此处的#cols#为点位符
            var sql = $"update {gParams.TableName} set #cols# where {WrapColumnEqParameter(provider, gParams.PkCol)}";
            return BuildStatement(CUDStatementName.Update, sql, gParams.Map);
        }

        public void Generate(SqlMap map, Type entityType)
        {
            if (statementList.Count > 0)
            {
                statementList.Clear();
                _statementListCache = null;
            }

            var gParams = new GeneratorParams(map, entityType);
            if (GeneratorFuncList != null && GeneratorFuncList.Count > 0)
            {
                foreach (var item in GeneratorFuncList)
                {
                    statementList.Add(item.Key, item.Value(gParams));
                }
            }
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
