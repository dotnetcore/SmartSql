using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SmartSql.Configuration;
using SmartSql.Configuration.Tags;
using SmartSql.Exceptions;
using SmartSql.IdGenerator;
using SmartSql.Reflection;
using SmartSql.Utils;

namespace SmartSql.ConfigBuilder
{
    public abstract class AbstractConfigBuilder : IConfigBuilder
    {
        protected IIdGeneratorBuilder IdGeneratorBuilder { get; set; } = new IdGeneratorBuilder();
        protected ILogger Logger { get; set; } = NullLogger.Instance;

        protected abstract void OnBeforeBuild();
        protected abstract void OnAfterBuild();
        public bool Initialized { get; private set; }
        public SmartSqlConfig SmartSqlConfig { get; private set; }
        public IConfigBuilder Parent { get; private set; }

        public virtual SmartSqlConfig Build()
        {
            if (Initialized)
            {
                return SmartSqlConfig;
            }

            SmartSqlConfig = Parent == null ? new SmartSqlConfig() : Parent.Build();
            
            OnBeforeBuild();
            BuildSettings();
            BuildProperties();
            BuildIdGenerators();
            BuildDatabase();
            BuildTypeHandlers();
            BuildTagBuilders();
            BuildAutoConverters();
            BuildSqlMaps();
            EnsureDependency();
            OnAfterBuild();
            Initialized = true;
            return SmartSqlConfig;
        }

        public void SetParent(IConfigBuilder configBuilder)
        {
            if (configBuilder == this)
            {
                throw new SmartSqlException("ConfigBuilder.Parent can't be self!");
            }

            Parent = configBuilder;
        }

        protected abstract void BuildSqlMaps();
        protected abstract void BuildTagBuilders();
        protected abstract void BuildTypeHandlers();
        protected abstract void BuildDatabase();
        protected abstract void BuildIdGenerators();
        protected abstract void BuildProperties();
        protected abstract void BuildSettings();
        protected abstract void BuildAutoConverters();
        public abstract void Dispose();

        protected void RegisterTypeHandler(TypeHandler typeHandlerConfig)
        {
            SmartSqlConfig.TypeHandlerFactory.Register(typeHandlerConfig);
        }

        protected void RegisterTagBuilder(string name, string type)
        {
            var tagBuilderType = TypeUtils.GetType(type);
            var tagBuilderObj =
                SmartSqlConfig.ObjectFactoryBuilder.GetObjectFactory(tagBuilderType, Type.EmptyTypes)(null) as
                    ITagBuilder;
            SmartSqlConfig.TagBuilderFactory.Register(name, tagBuilderObj);
        }

        protected void BuildSqlMap(ResourceType resourceType, string path)
        {
            switch (resourceType)
            {
                case ResourceType.Embedded:
                case ResourceType.File:
                {
                    var sqlMapXml = ResourceUtil.LoadAsXml(resourceType, path);
                    BuildSqlMap(sqlMapXml);
                    break;
                }
                case ResourceType.Directory:
                case ResourceType.DirectoryWithAllSub:
                {
                    SearchOption searchOption = SearchOption.TopDirectoryOnly;
                    if (resourceType == ResourceType.DirectoryWithAllSub)
                    {
                        searchOption = SearchOption.AllDirectories;
                    }

                    var dicPath = Path.Combine(AppContext.BaseDirectory, path);
                    var childSqlMapSources = Directory.EnumerateFiles(dicPath, "*.xml", searchOption);
                    foreach (var sqlMapPath in childSqlMapSources)
                    {
                        if (Logger.IsEnabled(LogLevel.Debug))
                        {
                            Logger.LogDebug($"BuildSqlMap.Child ->> Path :[{sqlMapPath}].");
                        }

                        var sqlMapXml = ResourceUtil.LoadFileAsXml(sqlMapPath);
                        BuildSqlMap(sqlMapXml);
                    }

                    break;
                }
            }
        }

        protected void BuildSqlMap(XmlDocument sqlMapXml)
        {
            SqlMapBuilder sqlMapXmlParser = new SqlMapBuilder(sqlMapXml, SmartSqlConfig);
            sqlMapXmlParser.Build();
        }

        #region 6. EnsureDependency

        private SqlMap EnsureSqlMap(string scope)
        {
            if (!SmartSqlConfig.SqlMaps.TryGetValue(scope, out SqlMap sqlMap))
            {
                throw new SmartSqlException($"ConfigLoader can not find SqlMap.Scoe:{scope}");
            }

            return sqlMap;
        }

        private ResultMap EnsureResultMap(string fullMapId)
        {
            var scope = fullMapId.Split('.')[0];
            var sqlMap = EnsureSqlMap(scope);
            if (!sqlMap.ResultMaps.TryGetValue(fullMapId, out ResultMap resultMap))
            {
                throw new SmartSqlException($"ConfigLoader can not find ResultMap.Id:{fullMapId}");
            }

            return resultMap;
        }

        /// <summary>
        /// 初始化依赖And检测循环依赖
        /// </summary>
        protected void EnsureDependency()
        {
            #region Init Statement.Include

            foreach (var sqlMap in SmartSqlConfig.SqlMaps)
            {
                foreach (var statementKV in sqlMap.Value.Statements)
                {
                    foreach (var include in statementKV.Value.IncludeDependencies)
                    {
                        if (include.RefId == include.Statement.FullSqlId)
                        {
                            throw new SmartSqlException($"Include.RefId can not be self statement.id:{include.RefId}");
                        }

                        var scope = include.RefId.Split('.')[0];

                        var refSqlMap = EnsureSqlMap(scope);
                        if (!refSqlMap.Statements.TryGetValue(include.RefId, out Statement refStatement))
                        {
                            throw new SmartSqlException($"Include can not find statement.id:{include.RefId}");
                        }

                        include.Ref = refStatement;
                        include.ChildTags = refStatement.SqlTags;
                    }
                }
            }

            #endregion

            #region Check Statement.Include Cyclic Dependency

            foreach (var sqlMap in SmartSqlConfig.SqlMaps)
            {
                foreach (var statementKV in sqlMap.Value.Statements)
                {
                    CheckIncludeCyclicDependency(statementKV.Value, statementKV.Value.IncludeDependencies);
                }
            }

            #endregion

            foreach (var sqlMapKV in SmartSqlConfig.SqlMaps)
            {
                var sqlMap = sqlMapKV.Value;

                #region Init MultipleResultMaps

                foreach (var mResultKV in sqlMap.MultipleResultMaps)
                {
                    var mResult = mResultKV.Value;
                    if (!String.IsNullOrEmpty(mResult.Root?.MapId))
                    {
                        mResult.Root.Map = EnsureResultMap(mResult.Root.MapId);
                    }

                    foreach (var result in mResult.Results)
                    {
                        if (String.IsNullOrEmpty(result?.MapId))
                        {
                            continue;
                        }

                        result.Map = EnsureResultMap(result.MapId);
                    }
                }

                #endregion

                #region Init Statement Attribute For Cache & ResultMap & ParameterMap & MultipleResultMap

                foreach (var statementKV in sqlMap.Statements)
                {
                    var statement = statementKV.Value;
                    if (!String.IsNullOrEmpty(statement.CacheId))
                    {
                        var scope = statement.CacheId.Split('.')[0];
                        if (!EnsureSqlMap(scope).Caches.TryGetValue(statement.CacheId, out Configuration.Cache cache))
                        {
                            throw new SmartSqlException(
                                $"Statement.Id:{statement.FullSqlId} can not find Cache.Id:{statement.CacheId}");
                        }

                        statement.Cache = cache;
                    }

                    if (!String.IsNullOrEmpty(statement.ParameterMapId))
                    {
                        var scope = statement.ParameterMapId.Split('.')[0];
                        if (!EnsureSqlMap(scope).ParameterMaps
                            .TryGetValue(statement.ParameterMapId, out ParameterMap parameterMap))
                        {
                            throw new SmartSqlException(
                                $"Statement.Id:{statement.FullSqlId} can not find ParameterMap.Id:{statement.ParameterMapId}");
                        }

                        statement.ParameterMap = parameterMap;
                    }

                    if (!String.IsNullOrEmpty(statement.ResultMapId))
                    {
                        var scope = statement.ResultMapId.Split('.')[0];
                        if (!EnsureSqlMap(scope).ResultMaps.TryGetValue(statement.ResultMapId, out ResultMap resultMap))
                        {
                            throw new SmartSqlException(
                                $"Statement.Id:{statement.FullSqlId} can not find ResultMap.Id:{statement.ResultMapId}");
                        }

                        statement.ResultMap = resultMap;
                    }

                    if (!String.IsNullOrEmpty(statement.MultipleResultMapId))
                    {
                        var scope = statement.MultipleResultMapId.Split('.')[0];
                        if (!EnsureSqlMap(scope).MultipleResultMaps.TryGetValue(statement.MultipleResultMapId,
                            out MultipleResultMap multipleResultMap))
                        {
                            throw new SmartSqlException(
                                $"Statement.Id:{statement.FullSqlId} can not find MultipleResultMap.Id:{statement.MultipleResultMapId}");
                        }

                        statement.MultipleResultMap = multipleResultMap;
                    }
                }

                #endregion
            }

            #region Init Statement.SqlCommandType

            foreach (var sqlMapKV in SmartSqlConfig.SqlMaps)
            {
                foreach (var statementKV in sqlMapKV.Value.Statements)
                {
                    var statement = statementKV.Value;
                    var fullSqlTextBuilder = new StringBuilder();
                    BuildStatementFullSql(statement, fullSqlTextBuilder);
                    var fullSqlText = fullSqlTextBuilder.ToString();
                    statement.StatementType = SmartSqlConfig.StatementAnalyzer.Analyse(fullSqlText);
                }
            }

            #endregion
        }

        private void CheckIncludeCyclicDependency(Statement statement, IEnumerable<Include> dependencies)
        {
            foreach (var dependency in dependencies)
            {
                if (statement == dependency.Ref)
                {
                    string errMsg =
                        $"Detecting Statement.Id:{statement.FullSqlId} and Statement.Id:{dependency.Statement.FullSqlId} have cyclic dependency!";
                    throw new SmartSqlException(errMsg);
                }

                CheckIncludeCyclicDependency(statement, dependency.Ref.IncludeDependencies);
            }
        }

        #region  BuildStatementFullSql

        private void BuildStatementFullSql(Statement statement, StringBuilder fullSqlTextBuilder)
        {
            foreach (var tag in statement.SqlTags)
            {
                if (tag is Include include)
                {
                    BuildStatementFullSql(include.Ref, fullSqlTextBuilder);
                }
                else
                {
                    BuildTagFullSql(statement, tag, fullSqlTextBuilder);
                }
            }
        }

        private void BuildTagFullSql(Statement statement, ITag tag, StringBuilder fullSqlTextBuilder)
        {
            if (tag is SqlText sqlText)
            {
                fullSqlTextBuilder.Append(sqlText.BodyText);
                return;
            }

            if (tag is Tag parentTag && parentTag.ChildTags != null)
            {
                foreach (var childTag in parentTag.ChildTags)
                {
                    BuildTagFullSql(statement, childTag, fullSqlTextBuilder);
                }
            }
        }

        #endregion

        #endregion
    }
}