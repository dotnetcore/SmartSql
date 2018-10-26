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
                SqlMapConfig.SmartSqlMaps = new Dictionary<String, SmartSqlMap> { };
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
                    Statements = new Dictionary<String, Statement> { },
                    Caches = new Dictionary<String, Configuration.Cache> { },
                    ResultMaps = new Dictionary<String, ResultMap> { },
                    MultipleResultMaps = new Dictionary<String, MultipleResultMap> { },
                    ParameterMaps = new Dictionary<String, ParameterMap> { }
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
                    sqlMap.Caches.Add(cache.Id, cache);
                }
                #endregion

                #region Init ResultMaps
                var resultMapsNodes = xmlDoc.SelectNodes("//ns:ResultMaps/ns:ResultMap", xmlNsM);
                foreach (XmlElement xmlNode in resultMapsNodes)
                {
                    var resultMap = MapFactory.LoadResultMap(xmlNode, sqlMap, xmlNsM);
                    sqlMap.ResultMaps.Add(resultMap.Id, resultMap);
                }
                #endregion
                #region Init MultipleResultMaps
                var multipleResultMapsNode = xmlDoc.SelectNodes("//ns:MultipleResultMaps/ns:MultipleResultMap", xmlNsM);
                foreach (XmlElement xmlNode in multipleResultMapsNode)
                {
                    var multipleResultMap = MapFactory.LoadMultipleResultMap(xmlNode, sqlMap);
                    sqlMap.MultipleResultMaps.Add(multipleResultMap.Id, multipleResultMap);
                }
                #endregion
                #region Init ParameterMaps
                var parameterMaps = xmlDoc.SelectNodes("//ns:ParameterMaps/ns:ParameterMap", xmlNsM);
                foreach (XmlElement xmlNode in parameterMaps)
                {
                    var parameterMap = MapFactory.LoadParameterMap(xmlNode, sqlMap);
                    sqlMap.ParameterMaps.Add(parameterMap.Id, parameterMap);
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
            var sqlMap = GetSmartSqlMap(scope);
            if (!sqlMap.ResultMaps.TryGetValue(fullMapId, out ResultMap resultMap))
            {
                throw new SmartSqlException($"ConfigLoader can not find ResultMap.Id:{fullMapId}");
            }
            return resultMap;
        }
        private SmartSqlMap GetSmartSqlMap(string scope)
        {
            if (!SqlMapConfig.SmartSqlMaps.TryGetValue(scope, out SmartSqlMap sqlMap))
            {
                throw new SmartSqlException($"ConfigLoader can not find SqlMap.Scoe:{scope}");
            }
            return sqlMap;
        }
        public void InitDependency()
        {
            #region Init Statement.Include
            foreach (var sqlMap in SqlMapConfig.SmartSqlMaps)
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
                        var refSqlMap = GetSmartSqlMap(scope);
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
            foreach (var sqlMap in SqlMapConfig.SmartSqlMaps)
            {
                foreach (var statementKV in sqlMap.Value.Statements)
                {
                    CheckIncludeCyclicDependency(statementKV.Value, statementKV.Value.IncludeDependencies);
                }
            }
            #endregion
            foreach (var sqlMapKV in SqlMapConfig.SmartSqlMaps)
            {
                var sqlMap = sqlMapKV.Value;
                #region Init MultipleResultMaps
                foreach (var mResultKV in sqlMap.MultipleResultMaps)
                {
                    var mResult = mResultKV.Value;
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
                foreach (var statementKV in sqlMap.Statements)
                {
                    var statement = statementKV.Value;
                    if (!String.IsNullOrEmpty(statement.CacheId))
                    {
                        var scope = statement.CacheId.Split('.')[0];
                        if (!GetSmartSqlMap(scope).Caches.TryGetValue(statement.CacheId, out Configuration.Cache cache))
                        {
                            throw new SmartSqlException($"Statement.Id:{statement.FullSqlId} can not find Cache.Id:{statement.CacheId}");
                        }
                        statement.Cache = cache;
                    }

                    if (!String.IsNullOrEmpty(statement.ResultMapId))
                    {
                        var scope = statement.ResultMapId.Split('.')[0];
                        if (!GetSmartSqlMap(scope).ResultMaps.TryGetValue(statement.ResultMapId, out ResultMap resultMap))
                        {
                            throw new SmartSqlException($"Statement.Id:{statement.FullSqlId} can not find ResultMap.Id:{statement.ResultMapId}");
                        }
                        statement.ResultMap = resultMap;
                    }

                    if (!String.IsNullOrEmpty(statement.ParameterMapId))
                    {
                        var scope = statement.ParameterMapId.Split('.')[0];
                        if (!GetSmartSqlMap(scope).ParameterMaps.TryGetValue(statement.ParameterMapId, out ParameterMap parameterMap))
                        {
                            throw new SmartSqlException($"Statement.Id:{statement.FullSqlId} can not find ParameterMap.Id:{statement.ParameterMapId}");
                        }
                        statement.ParameterMap = parameterMap;
                    }

                    if (!String.IsNullOrEmpty(statement.MultipleResultMapId))
                    {
                        var scope = statement.MultipleResultMapId.Split('.')[0];
                        if (!GetSmartSqlMap(scope).MultipleResultMaps.TryGetValue(statement.MultipleResultMapId, out MultipleResultMap multipleResultMap))
                        {
                            throw new SmartSqlException($"Statement.Id:{statement.FullSqlId} can not find MultipleResultMap.Id:{statement.MultipleResultMapId}");
                        }
                        statement.MultipleResultMap = multipleResultMap;
                    }
                }
                #endregion
            }
            #region Init Statement.SqlCommandType
            foreach (var sqlMapKV in SqlMapConfig.SmartSqlMaps)
            {
                foreach (var statementKV in sqlMapKV.Value.Statements)
                {
                    var statement = statementKV.Value;
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
                sqlMap.Statements.Add(statement.FullSqlId, statement);
            }
        }
    }

    public class ConfigStream
    {
        public String Path { get; set; }
        public Stream Stream { get; set; }
    }
}
