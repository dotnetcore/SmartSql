using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using SmartSql.Configuration;
using SmartSql.Configuration.Statements;
using SmartSql.Configuration.Maps;
using System.Linq;
using SmartSql.Exceptions;
using SmartSql.Configuration.Tags;
using System.Text;
using SmartSql.Utils;

namespace SmartSql.Abstractions.Config
{
    public abstract class ConfigLoader : IConfigLoader
    {
        private StatementFactory _statementFactory = new StatementFactory();
        private SqlCommandAnalyzer _sqlCommandAnalyzer = new SqlCommandAnalyzer();
        public SmartSqlMapConfig SqlMapConfig { get; set; }

        public abstract event OnChangedHandler OnChanged;

        public abstract void Dispose();
        public abstract SmartSqlMapConfig Load();

        public SmartSqlMapConfig LoadConfig(ConfigStream configStream)
        {
            using (configStream.Stream)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(SmartSqlMapConfig));
                SqlMapConfig = xmlSerializer.Deserialize(configStream.Stream) as SmartSqlMapConfig;
                SqlMapConfig.Path = configStream.Path;
                SqlMapConfig.SmartSqlMaps = new List<SmartSqlMap> { };
                if (SqlMapConfig.TypeHandlers != null)
                {
                    foreach (var typeHandler in SqlMapConfig.TypeHandlers)
                    {
                        typeHandler.Handler = TypeHandlerFactory.Create(typeHandler.Type);
                    }
                }
                return SqlMapConfig;
            }
        }
        public SmartSqlMap LoadSmartSqlMap(ConfigStream configStream)
        {
            using (configStream.Stream)
            {
                var sqlMap = new SmartSqlMap
                {
                    SqlMapConfig = SqlMapConfig,
                    Path = configStream.Path,
                    Statements = new List<Statement> { },
                    Caches = new List<Configuration.Cache> { },
                    ResultMaps = new List<ResultMap> { },
                    MultipleResultMaps = new List<MultipleResultMap> { },
                    ParameterMaps = new List<ParameterMap> { }
                };
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(configStream.Stream);
                XmlNamespaceManager xmlNsM = new XmlNamespaceManager(xmlDoc.NameTable);
                xmlNsM.AddNamespace("ns", "http://SmartSql.net/schemas/SmartSqlMap.xsd");
                sqlMap.Scope = xmlDoc.SelectSingleNode("//ns:SmartSqlMap", xmlNsM)
                    .Attributes["Scope"].Value;

                #region Init Caches
                var cacheNodes = xmlDoc.SelectNodes("//ns:Caches/ns:Cache", xmlNsM);
                foreach (XmlElement cacheNode in cacheNodes)
                {
                    var cache = CacheFactory.Load(cacheNode, sqlMap);
                    sqlMap.Caches.Add(cache);
                }
                #endregion

                #region Init ResultMaps
                var resultMapsNodes = xmlDoc.SelectNodes("//ns:ResultMaps/ns:ResultMap", xmlNsM);
                foreach (XmlElement xmlNode in resultMapsNodes)
                {
                    var resultMap = MapFactory.LoadResultMap(xmlNode, sqlMap, xmlNsM);
                    sqlMap.ResultMaps.Add(resultMap);
                }
                #endregion
                #region Init MultipleResultMaps
                var multipleResultMapsNode = xmlDoc.SelectNodes("//ns:MultipleResultMaps/ns:MultipleResultMap", xmlNsM);
                foreach (XmlElement xmlNode in multipleResultMapsNode)
                {
                    var multipleResultMap = MapFactory.LoadMultipleResultMap(xmlNode, sqlMap);
                    sqlMap.MultipleResultMaps.Add(multipleResultMap);
                }
                #endregion
                #region Init ParameterMaps
                var parameterMaps = xmlDoc.SelectNodes("//ns:ParameterMaps/ns:ParameterMap", xmlNsM);
                foreach (XmlElement xmlNode in parameterMaps)
                {
                    var parameterMap = MapFactory.LoadParameterMap(xmlNode, sqlMap);
                    sqlMap.ParameterMaps.Add(parameterMap);
                }
                #endregion

                #region Init Statement
                var statementNodes = xmlDoc.SelectNodes("//ns:Statements/ns:Statement", xmlNsM);
                LoadStatementInSqlMap(sqlMap, statementNodes);
                #endregion

                return sqlMap;
            }
        }
        private ResultMap GetResultMap(string fullMapId)
        {
            var scope = fullMapId.Split('.')[0];
            var sqlMap = SqlMapConfig.SmartSqlMaps.FirstOrDefault(m => m.Scope == scope);
            if (sqlMap == null) { throw new SmartSqlException($"ConfigLoader can not find SqlMap.Scoe:{scope}"); }
            var resultMap = sqlMap.ResultMaps.FirstOrDefault(m => m.Id == fullMapId);
            if (resultMap == null) { throw new SmartSqlException($"ConfigLoader can not find ResultMap.Id:{fullMapId}"); }
            return resultMap;
        }
        private SmartSqlMap GetSmartSqlMap(string scope)
        {
            return SqlMapConfig.SmartSqlMaps.FirstOrDefault(m => m.Scope == scope);
        }
        public void InitDependency()
        {
            #region Init Statement.Include
            foreach (var sqlMap in SqlMapConfig.SmartSqlMaps)
            {
                foreach (var statement in sqlMap.Statements)
                {
                    foreach (var include in statement.IncludeDependencies)
                    {
                        if (include.RefId == include.Statement.FullSqlId)
                        {
                            throw new SmartSqlException($"Include.RefId can not be self statement.id:{include.RefId}");
                        }
                        var scope = include.RefId.Split('.')[0];
                        var refStatement = GetSmartSqlMap(scope)?.Statements?.FirstOrDefault(m => m.FullSqlId == include.RefId);
                        include.Ref = refStatement ?? throw new SmartSqlException($"Include can not find statement.id:{include.RefId}");
                    }
                }
            }
            #endregion
            #region Check Statement.Include Cyclic Dependency
            foreach (var sqlMap in SqlMapConfig.SmartSqlMaps)
            {
                foreach (var statement in sqlMap.Statements)
                {
                    CheckIncludeCyclicDependency(statement, statement.IncludeDependencies);
                }
            }
            #endregion
            foreach (var sqlMap in SqlMapConfig.SmartSqlMaps)
            {
                #region Init MultipleResultMaps
                foreach (var mResult in sqlMap.MultipleResultMaps)
                {
                    if (!String.IsNullOrEmpty(mResult.Root?.MapId))
                    {
                        mResult.Root.Map = GetResultMap(mResult.Root.MapId);
                    }
                    foreach (var result in mResult.Results)
                    {
                        if (String.IsNullOrEmpty(result?.MapId))
                        {
                            continue;
                        }
                        result.Map = GetResultMap(result.MapId);
                    }
                }
                #endregion
                #region Init Statement Attribute For Cache & ResultMap & ParameterMap & MultipleResultMap
                foreach (var statement in sqlMap.Statements)
                {
                    if (!String.IsNullOrEmpty(statement.CacheId))
                    {
                        var scope = statement.CacheId.Split('.')[0];
                        var cache = GetSmartSqlMap(scope)?
                            .Caches?.FirstOrDefault(m => m.Id == statement.CacheId);
                        statement.Cache = cache ?? throw new SmartSqlException($"Statement.Id:{statement.FullSqlId} can not find Cache.Id:{statement.CacheId}");
                    }

                    if (!String.IsNullOrEmpty(statement.ResultMapId))
                    {
                        var scope = statement.ResultMapId.Split('.')[0];
                        var resultMap = GetSmartSqlMap(scope)?
                            .ResultMaps?.FirstOrDefault(m => m.Id == statement.ResultMapId);
                        statement.ResultMap = resultMap ?? throw new SmartSqlException($"Statement.Id:{statement.FullSqlId} can not find ResultMap.Id:{statement.ResultMapId}");
                    }

                    if (!String.IsNullOrEmpty(statement.ParameterMapId))
                    {
                        var scope = statement.ParameterMapId.Split('.')[0];
                        var parameterMap = GetSmartSqlMap(scope)?
                            .ParameterMaps?.FirstOrDefault(m => m.Id == statement.ParameterMapId);
                        statement.ParameterMap = parameterMap ?? throw new SmartSqlException($"Statement.Id:{statement.FullSqlId} can not find ParameterMap.Id:{statement.ParameterMapId}");
                    }

                    if (!String.IsNullOrEmpty(statement.MultipleResultMapId))
                    {
                        var scope = statement.MultipleResultMapId.Split('.')[0];
                        var multipleResultMap = GetSmartSqlMap(scope)?
                                .MultipleResultMaps?.FirstOrDefault(m => m.Id == statement.MultipleResultMapId);
                        statement.MultipleResultMap = multipleResultMap ?? throw new SmartSqlException($"Statement.Id:{statement.FullSqlId} can not find MultipleResultMap.Id:{statement.MultipleResultMapId}");
                    }
                }
                #endregion
            }
            #region Init Statement.SqlCommandType
            foreach (var sqlMap in SqlMapConfig.SmartSqlMaps)
            {
                foreach (var statement in sqlMap.Statements)
                {
                    var fullSqlTextBuilder = new StringBuilder();
                    BuildStatementFullSql(statement, fullSqlTextBuilder);
                    var fullSqlText = fullSqlTextBuilder.ToString();
                    statement.SqlCommandType = _sqlCommandAnalyzer.Analyse(fullSqlText);
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
        private void LoadStatementInSqlMap(SmartSqlMap sqlMap, XmlNodeList statementNodes)
        {
            foreach (XmlElement statementNode in statementNodes)
            {
                var statement = _statementFactory.Load(statementNode, sqlMap);
                sqlMap.Statements.Add(statement);
            }
        }
    }

    public class ConfigStream
    {
        public String Path { get; set; }
        public Stream Stream { get; set; }
    }
}
