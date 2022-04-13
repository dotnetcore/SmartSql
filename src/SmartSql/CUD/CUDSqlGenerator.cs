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
using System.Text;

namespace SmartSql.CUD
{
    public class CUDSqlGenerator : ICUDSqlGenerator
    {


        private IDictionary<string, Func<GeneratorParams, Statement>> _generatorFuncList;
        private DbProvider _provider;
        private StatementAnalyzer _analyzer;


        public CUDSqlGenerator(SmartSqlConfig config)
        {
            _generatorFuncList = new Dictionary<string, Func<GeneratorParams, Statement>>
            {
                { CUDStatementName.GetById, BuildGetEntity},
                { CUDStatementName.Insert, BuildInsert },
                { CUDStatementName.InsertReturnId, BuildInsertReturnId },
                { CUDStatementName.Update, BuildUpdate },
                { CUDStatementName.DeleteById, BuildDeleteById },
                { CUDStatementName.DeleteAll, BuildDeleteAll },
                { CUDStatementName.DeleteMany, BuildDeleteMany },
            };
            _provider = config.Database.DbProvider;
            _analyzer = new StatementAnalyzer();
        }



        private Statement BuildStatement(string statementId, string sql, SqlMap sqlMap)
        {
            return new Statement()
            {
                SqlMap = sqlMap,
                Id = statementId,
                CommandType = System.Data.CommandType.Text,
                StatementType = _analyzer.Analyse(sql),
                SqlTags = new List<ITag>
                    {
                        new SqlText(sql, _provider.ParameterPrefix)
                    },
            };
        }

        private Statement BuildStatement(string statementId, List<ITag> sqlTags, SqlMap sqlMap)
        {
            return new Statement()
            {
                SqlMap = sqlMap,
                Id = statementId,
                CommandType = System.Data.CommandType.Text,
                //StatementType = _analyzer.Analyse(sql),
                SqlTags = sqlTags,
            };
        }


        public Statement BuildGetEntity(GeneratorParams gParams)
        {
            var sql =
                $"select * From {gParams.TableName} Where {WrapColumnEqParameter(_provider, gParams.PkCol)}";

            return BuildStatement(CUDStatementName.GetById, sql, gParams.Map);
        }

        public Statement BuildDeleteAll(GeneratorParams gParams)
        {
            var sql = $"delete from {gParams.TableName}";
            return BuildStatement(CUDStatementName.DeleteAll, sql, gParams.Map);
        }

        public Statement BuildDeleteById(GeneratorParams gParams)
        {
            var sql =
                $"Delete From {gParams.TableName} Where {WrapColumnEqParameter(_provider, gParams.PkCol)}";

            return BuildStatement(CUDStatementName.DeleteById, sql, gParams.Map);
        }

        public Statement BuildDeleteMany(GeneratorParams gParams)
        {
            var sql = $"Delete From {gParams.TableName} Where {gParams.PkCol.Name} In {FormatParameterName(_provider, gParams.PkCol.Name)}";
            return BuildStatement(CUDStatementName.DeleteMany, sql, gParams.Map);
        }

        public Statement BuildInsert(GeneratorParams gParams)
        {
            var cols = gParams.ColumnMaps;
            string colNames = string.Empty;
            string colVals = string.Empty;
            foreach (var col in cols)
            {
                if (col.Value.IsAutoIncrement)
                    continue;
                colNames += $",{FormatColumnName(_provider, col.Value.Name)}";
                colVals += $",{FormatParameterName(_provider, col.Value.Property.Name)}";
            }
            var sql = $"insert into {gParams.TableName} ({colNames.Substring(1)}) values ({colVals.Substring(1)})";
            return BuildStatement(CUDStatementName.Insert, sql, gParams.Map);
        }

        public Statement BuildInsertReturnId(GeneratorParams arg)
        {
            var statement = BuildInsert(arg);

            if (_provider.Type == DbProviderManager.POSTGRESQL_DBPROVIDER.Type)
            {
                statement.SqlTags.Add(new SqlText($" ; Returning {arg.PkCol.Name};", _provider.ParameterPrefix));
            }
            else
            {
                statement.SqlTags.Add(new SqlText($"; {_provider.SelectAutoIncrement};", _provider.ParameterPrefix));
            }

            return statement;
        }

        public Statement BuildUpdate(GeneratorParams gParams)
        {
            var dbPrefix = _provider.ParameterPrefix;

            var updateTags = new List<ITag>();
            foreach (var col in gParams.ColumnMaps.Values)
            {
                if (col.IsPrimaryKey)
                    continue;
                var p = new IsProperty()
                {
                    Prepend = ",",
                    Property = col.Property.Name,
                    ChildTags = new List<ITag>()
                    {
                        new SqlText($"{WrapColumnEqParameter(_provider,col)}", dbPrefix)
                    }
                };
                updateTags.Add(p);
            }

            var sqlTags = new List<ITag>()
            {
                new SqlText($"update {gParams.TableName}", dbPrefix),
                new Set()
                {
                    ChildTags = updateTags
                },
                new SqlText($" where {WrapColumnEqParameter(_provider, gParams.PkCol)}", dbPrefix),
            };
            
            return BuildStatement(CUDStatementName.Update, sqlTags, gParams.Map);
        }

        public IDictionary<string, Statement> Generate(SqlMap map, Type entityType)
        {
            var statementList = new Dictionary<string, Statement>();

            var gParams = new GeneratorParams(map, entityType);
            if (_generatorFuncList != null && _generatorFuncList.Count > 0)
            {
                foreach (var item in _generatorFuncList)
                {
                    statementList.Add($"{map.Scope}.{item.Key}", item.Value(gParams));
                }
            }
            return statementList;
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
