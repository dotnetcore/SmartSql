using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Configuration;
using SmartSql.Configuration.Tags;
using SmartSql.Utils;
using System.Xml;
using SmartSql.DataSource;
using SmartSql.Exceptions;
using SmartSql.Reflection;
using System.IO;
using SmartSql.IdGenerator;

namespace SmartSql.ConfigBuilder
{
    public class XmlConfigBuilder : IConfigBuilder
    {
        public const String SMART_SQL_CONFIG_NAMESPACE = "http://SmartSql.net/schemas/SmartSqlMapConfig-v4.xsd";
        public const String CONFIG_PREFIX = "Config";
        public const String TYPE_ATTRIBUTE = "Type";
        protected XmlDocument XmlConfig { get; }
        protected XmlNamespaceManager XmlNsManager { get; }
        protected XmlNode XmlConfigRoot { get; }
        protected SmartSqlConfig SmartSqlConfig { get; set; }
        private readonly IIdGeneratorBuilder _idGeneratorBuilder = new IdGeneratorBuilder();
        public XmlConfigBuilder(ResourceType resourceType, string resourcePath)
        {
            XmlConfig = ResourceUtil.LoadAsXml(resourceType, resourcePath);
            XmlNsManager = new XmlNamespaceManager(XmlConfig.NameTable);
            XmlNsManager.AddNamespace(CONFIG_PREFIX, SMART_SQL_CONFIG_NAMESPACE);
            XmlConfigRoot = XmlConfig.SelectSingleNode($"/{CONFIG_PREFIX}:SmartSqlMapConfig", XmlNsManager);
        }

        public void Dispose()
        {

        }

        protected void InitDefault()
        {
            SmartSqlConfig = new SmartSqlConfig();
        }

        public SmartSqlConfig Build()
        {
            InitDefault();
            BuildSettings();
            BuildProperties();
            BuildIdGenerator();
            BuildDatabase();
            BuildTypeHandlers();
            BuildTagBuilders();
            BuildSqlMaps();
            if (SmartSqlConfig.StatementAnalyzer == null)
            {
                SmartSqlConfig.StatementAnalyzer = new StatementAnalyzer();
            }
            if (SmartSqlConfig.SqlParamAnalyzer == null)
            {
                SmartSqlConfig.SqlParamAnalyzer = new SqlParamAnalyzer(SmartSqlConfig.Settings.IgnoreParameterCase, SmartSqlConfig.Database.DbProvider.ParameterPrefix);
            }
            EnsureDependency();
            return SmartSqlConfig;
        }
        #region  0. Settings
        protected void BuildSettings()
        {
            var settingsXPath = $"{CONFIG_PREFIX}:Settings";
            var settingsNode = XmlConfigRoot.SelectSingleNode(settingsXPath, XmlNsManager);
            if (settingsNode == null)
            {
                return;
            }
            if (settingsNode.Attributes.TryGetValueAsBoolean(nameof(Settings.IgnoreParameterCase),
                    out bool ignoreParameterCase))
            {
                SmartSqlConfig.Settings.IgnoreParameterCase = ignoreParameterCase;
            }
            if (settingsNode.Attributes.TryGetValueAsBoolean(nameof(Settings.IsCacheEnabled),
                out bool isCacheEnabled))
            {
                SmartSqlConfig.Settings.IsCacheEnabled = isCacheEnabled;
            }
            if (settingsNode.Attributes.TryGetValueAsString(nameof(Settings.ParameterPrefix),
                out string parameterPrefix))
            {
                SmartSqlConfig.Settings.ParameterPrefix = parameterPrefix;
            }
        }
        #endregion
        #region 1. Properties
        private void BuildProperties()
        {
            SmartSqlConfig.Properties = new Properties();
            var properties = ParseProperties(XmlConfigRoot);
            SmartSqlConfig.Properties.Load(properties);
        }
        #endregion
        #region 2. IdGen
        private void BuildIdGenerator()
        {
            var settingsXPath = $"{CONFIG_PREFIX}:IdGenerator";
            var idGeneratorNode = XmlConfigRoot.SelectSingleNode(settingsXPath, XmlNsManager);
            if (idGeneratorNode == null)
            {
                return;
            }

            if (!idGeneratorNode.Attributes.TryGetValueAsString("Type", out string typeString, SmartSqlConfig.Properties))
            {
                throw new SmartSqlException("IdGenerator.Type can not be null.");
            }
            var parameters = ParseProperties(idGeneratorNode);
            SmartSqlConfig.IdGenerator = _idGeneratorBuilder.Build(typeString, parameters);
        }
        #endregion
        #region 2. Database
        protected void BuildDatabase()
        {
            Database database = new Database
            {
                Reads = new Dictionary<string, ReadDataSource>()
            };
            SmartSqlConfig.Database = database;
            var databaseXPath = $"{CONFIG_PREFIX}:Database";
            var databaseNode = XmlConfigRoot.SelectSingleNode(databaseXPath, XmlNsManager);
            if (databaseNode == null)
            {
                throw new SmartSqlException("Can not find Database Node.");
            }
            var dbProviderNode = databaseNode.SelectSingleNode($"{CONFIG_PREFIX}:DbProvider", XmlNsManager);
            database.DbProvider = ParseDbProvider(dbProviderNode);
            var writeDataSourceNode = databaseNode.SelectSingleNode($"{CONFIG_PREFIX}:Write", XmlNsManager);
            database.Write = ParseWriteDataSource(writeDataSourceNode);
            database.Write.DbProvider = database.DbProvider;
            var readDataSourceNodes = databaseNode.SelectNodes($"{CONFIG_PREFIX}:Read", XmlNsManager);
            if (readDataSourceNodes != null)
            {
                foreach (XmlNode readNode in readDataSourceNodes)
                {
                    var readDb = ParseReadDataSource(readNode);
                    readDb.DbProvider = database.DbProvider;
                    database.Reads.Add(readDb.Name, readDb);
                }
            }
        }

        private DbProvider ParseDbProvider(XmlNode dbProviderNode)
        {
            if (dbProviderNode == null)
            {
                throw new SmartSqlException("DbProvider can not be null.");
            }
            DbProvider dbProvider = new DbProvider();
            if (dbProviderNode.Attributes.TryGetValueAsString(nameof(DbProvider.Name), out string name, SmartSqlConfig.Properties))
            {
                dbProvider.Name = name;
            }
            if (dbProviderNode.Attributes.TryGetValueAsString(nameof(DbProvider.ParameterPrefix), out string parameterPrefix, SmartSqlConfig.Properties))
            {
                dbProvider.ParameterPrefix = parameterPrefix;
            }
            if (dbProviderNode.Attributes.TryGetValueAsString(nameof(DbProvider.ParameterNamePrefix), out string namePrefix, SmartSqlConfig.Properties))
            {
                dbProvider.ParameterNamePrefix = namePrefix;
            }
            if (dbProviderNode.Attributes.TryGetValueAsString(nameof(DbProvider.ParameterNameSuffix), out string nameSuffix, SmartSqlConfig.Properties))
            {
                dbProvider.ParameterNameSuffix = nameSuffix;
            }
            if (dbProviderNode.Attributes.TryGetValueAsString(nameof(DbProvider.SelectAutoIncrement), out string selectAutoIncrement, SmartSqlConfig.Properties))
            {
                dbProvider.SelectAutoIncrement = selectAutoIncrement;
            }

            if (dbProviderNode.Attributes.TryGetValueAsString(TYPE_ATTRIBUTE, out string type, SmartSqlConfig.Properties)
            )
            {
                dbProvider.Type = type;
            }

            if (!String.IsNullOrEmpty(dbProvider.Type))
            {
                dbProvider.Factory = DbProviderManager.Instance.GetDbProviderFactory(dbProvider.Type);
            }
            else
            {
                if (!DbProviderManager.Instance.TryInit(ref dbProvider))
                {
                    throw new SmartSqlException("DbProvider can not be Init.");
                }
            }

            return dbProvider;
        }
        private WriteDataSource ParseWriteDataSource(XmlNode writeDataSourceNode)
        {
            if (writeDataSourceNode == null)
            {
                throw new SmartSqlException("WriteDataSource can not be null.");
            }
            var writeDataSource = new WriteDataSource();
            if (writeDataSourceNode.Attributes.TryGetValueAsString(nameof(WriteDataSource.Name), out string name, SmartSqlConfig.Properties)
            )
            {
                writeDataSource.Name = name;
            }
            if (writeDataSourceNode.Attributes.TryGetValueAsString(nameof(WriteDataSource.ConnectionString), out string connectionString, SmartSqlConfig.Properties)
            )
            {
                writeDataSource.ConnectionString = connectionString;
            }

            return writeDataSource;
        }
        private ReadDataSource ParseReadDataSource(XmlNode readDataSourceNode)
        {
            if (readDataSourceNode == null)
            {
                throw new SmartSqlException("ReadDataSource can not be null.");
            }
            var readDataSource = new ReadDataSource();
            if (readDataSourceNode.Attributes.TryGetValueAsString(nameof(ReadDataSource.Name), out string name, SmartSqlConfig.Properties)
            )
            {
                readDataSource.Name = name;
            }
            if (readDataSourceNode.Attributes.TryGetValueAsString(nameof(ReadDataSource.ConnectionString), out string connectionString, SmartSqlConfig.Properties)
            )
            {
                readDataSource.ConnectionString = connectionString;
            }
            if (readDataSourceNode.Attributes.TryGetValueAsInt32(nameof(ReadDataSource.Weight), out int weight, SmartSqlConfig.Properties)
            )
            {
                readDataSource.Weight = weight;
            }

            return readDataSource;
        }
        #endregion
        #region 3. TypeHandlers
        protected void BuildTypeHandlers()
        {
            var typeHandlerXPath = $"{CONFIG_PREFIX}:TypeHandlers/{CONFIG_PREFIX}:TypeHandler";
            var typeHandlerNodes = XmlConfigRoot.SelectNodes(typeHandlerXPath, XmlNsManager);
            if (typeHandlerNodes != null)
            {
                foreach (XmlNode typeHandlerNode in typeHandlerNodes)
                {
                    BuildTypeHandler(typeHandlerNode);
                }
            }
        }
        private void BuildTypeHandler(XmlNode typeHandlerNode)
        {
            TypeHandler typeHandlerConfig = new TypeHandler
            {
                Parameters = ParseProperties(typeHandlerNode)
            };
            typeHandlerNode.Attributes.TryGetValueAsString(nameof(TypeHandler.Name), out string handlerName, SmartSqlConfig.Properties);
            typeHandlerNode.Attributes.TryGetValueAsString("MappedType", out string csharpTypeStr, SmartSqlConfig.Properties);
            if (!typeHandlerNode.Attributes.TryGetValueAsString(TYPE_ATTRIBUTE, out string type, SmartSqlConfig.Properties))
            {
                throw new SmartSqlException("TypeHandler.Type can not be null.");
            }
            typeHandlerConfig.Name = handlerName;
            typeHandlerConfig.HandlerType = TypeUtils.GetType(type);

            if (typeHandlerConfig.HandlerType.IsGenericType)
            {
                if (String.IsNullOrEmpty(csharpTypeStr))
                {
                    throw new SmartSqlException("TypeHandler.MappedType can not be null.");
                }
                typeHandlerConfig.MappedType = TypeUtils.GetType(csharpTypeStr);
            }
            SmartSqlConfig.TypeHandlerFactory.Register(typeHandlerConfig);
        }


        #endregion
        #region 4. TagBuilders
        protected void BuildTagBuilders()
        {
            var tagBuilderXPath = $"{CONFIG_PREFIX}:TagBuilders/{CONFIG_PREFIX}:TagBuilder";
            var tagBuilderNodes = XmlConfigRoot.SelectNodes(tagBuilderXPath, XmlNsManager);
            if (tagBuilderNodes != null)
            {
                foreach (XmlNode tagBuilderNode in tagBuilderNodes)
                {
                    BuildTagBuilder(tagBuilderNode);
                }
            }
        }
        private void BuildTagBuilder(XmlNode tagBuilderNode)
        {
            if (!tagBuilderNode.Attributes.TryGetValueAsString("Name", out string name, SmartSqlConfig.Properties)
            )
            {
                throw new SmartSqlException("TagBuilder.Name can not be null.");
            }
            if (!tagBuilderNode.Attributes.TryGetValueAsString(TYPE_ATTRIBUTE, out string type, SmartSqlConfig.Properties)
            )
            {
                throw new SmartSqlException("TagBuilder.Type can not be null.");
            }

            var tagBuilderType = TypeUtils.GetType(type);
            var tagBuilderObj = SmartSqlConfig.ObjectFactoryBuilder.GetObjectFactory(tagBuilderType, Type.EmptyTypes)(null) as ITagBuilder;
            SmartSqlConfig.TagBuilderFactory.Register(name, tagBuilderObj);
        }
        #endregion
        #region 5. SmartSqlMaps
        protected void BuildSqlMaps()
        {
            var sqlMapXPath = $"{CONFIG_PREFIX}:SmartSqlMaps/{CONFIG_PREFIX}:SmartSqlMap";
            var sqlMapNodes = XmlConfigRoot.SelectNodes(sqlMapXPath, XmlNsManager);
            if (sqlMapNodes != null)
            {
                foreach (XmlNode sqlMapNode in sqlMapNodes)
                {
                    BuildSqlMap(sqlMapNode);
                }
            }
        }
        protected void BuildSqlMap(XmlNode sqlMapNode)
        {
            if (!sqlMapNode.Attributes.TryGetValueAsString(nameof(SqlMap.Path), out string path, SmartSqlConfig.Properties)
            )
            {
                throw new SmartSqlException("SmartSqlMap.Path can not be null.");
            }
            if (!sqlMapNode.Attributes.TryGetValueAsString(TYPE_ATTRIBUTE, out string type, SmartSqlConfig.Properties)
            )
            {
                throw new SmartSqlException("TagBuilder.Type can not be null.");
            }
            ResourceType resourceType = ResourceType.File;
            Enum.TryParse<ResourceType>(type, out resourceType);
            switch (resourceType)
            {
                case ResourceType.Embedded:
                case ResourceType.File:
                    {
                        var sqlMapXml = ResourceUtil.LoadAsXml(resourceType, path);
                        BuildSqlMap(sqlMapXml, SmartSqlConfig);
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
                            var sqlMapXml = ResourceUtil.LoadFileAsXml(sqlMapPath);
                            BuildSqlMap(sqlMapXml, SmartSqlConfig);
                        }
                        break;
                    }
            }
        }
        private void BuildSqlMap(XmlDocument sqlMapXml, SmartSqlConfig smartSqlConfig)
        {
            SqlMapBuilder sqlMapXmlParser = new SqlMapBuilder(sqlMapXml, smartSqlConfig);
            sqlMapXmlParser.Build();
        }
        #endregion
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
                            throw new SmartSqlException($"Statement.Id:{statement.FullSqlId} can not find Cache.Id:{statement.CacheId}");
                        }
                        statement.Cache = cache;
                    }

                    if (!String.IsNullOrEmpty(statement.ResultMapId))
                    {
                        var scope = statement.ResultMapId.Split('.')[0];
                        if (!EnsureSqlMap(scope).ResultMaps.TryGetValue(statement.ResultMapId, out ResultMap resultMap))
                        {
                            throw new SmartSqlException($"Statement.Id:{statement.FullSqlId} can not find ResultMap.Id:{statement.ResultMapId}");
                        }
                        statement.ResultMap = resultMap;
                    }

                    if (!String.IsNullOrEmpty(statement.MultipleResultMapId))
                    {
                        var scope = statement.MultipleResultMapId.Split('.')[0];
                        if (!EnsureSqlMap(scope).MultipleResultMaps.TryGetValue(statement.MultipleResultMapId, out MultipleResultMap multipleResultMap))
                        {
                            throw new SmartSqlException($"Statement.Id:{statement.FullSqlId} can not find MultipleResultMap.Id:{statement.MultipleResultMapId}");
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
                    string errMsg = $"Detecting Statement.Id:{statement.FullSqlId} and Statement.Id:{dependency.Statement.FullSqlId} have cyclic dependency!";
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
        private IDictionary<String, object> ParseProperties(XmlNode parentNode)
        {
            var parametersNode = parentNode.SelectSingleNode($"{CONFIG_PREFIX}:Properties", XmlNsManager);
            var parameters = new Dictionary<String, object>();
            if (parametersNode == null) { return parameters; }
            var paramNodes = parametersNode.SelectNodes($"{CONFIG_PREFIX}:Property", XmlNsManager);
            if (paramNodes != null && paramNodes.Count > 0)
            {
                foreach (XmlNode paramterNode in paramNodes)
                {
                    paramterNode.Attributes.TryGetValueAsString("Name", out var name, SmartSqlConfig.Properties);
                    paramterNode.Attributes.TryGetValueAsString("Value", out var propVal, SmartSqlConfig.Properties);
                    parameters.Add(name, propVal);
                }
            }
            return parameters;
        }
    }
}
