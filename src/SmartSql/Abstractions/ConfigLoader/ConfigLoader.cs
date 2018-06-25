using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Xml;
using System.Xml.Serialization;
using SmartSql.Configuration;
using SmartSql.Configuration.Statements;
using SmartSql.Configuration.Maps;

namespace SmartSql.Abstractions.Config
{
    public abstract class ConfigLoader : IConfigLoader
    {
        private StatementFactory _statementFactory = new StatementFactory();

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
                    ParameterMaps = new List<ParameterMap> { }
                };
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(configStream.Stream);
                XmlNamespaceManager xmlNsM = new XmlNamespaceManager(xmlDoc.NameTable);
                xmlNsM.AddNamespace("ns", "http://SmartSql.net/schemas/SmartSqlMap.xsd");
                sqlMap.Scope = xmlDoc.SelectSingleNode("//ns:SmartSqlMap", xmlNsM)
                    .Attributes["Scope"].Value;

                #region Init Caches
                var cacheNodes = xmlDoc.SelectNodes("//ns:Cache", xmlNsM);
                foreach (XmlElement cacheNode in cacheNodes)
                {
                    var cache = CacheFactory.Load(cacheNode);
                    sqlMap.Caches.Add(cache);
                }
                #endregion

                #region Init ResultMaps
                var resultMapsNodes = xmlDoc.SelectNodes("//ns:ResultMap", xmlNsM);
                foreach (XmlElement xmlNode in resultMapsNodes)
                {
                    var resultMap = MapFactory.LoadResultMap(xmlNode, SqlMapConfig, xmlNsM);
                    sqlMap.ResultMaps.Add(resultMap);
                }
                #endregion
                #region Init ParameterMaps
                var parameterMaps = xmlDoc.SelectNodes("//ns:ParameterMap", xmlNsM);
                foreach (XmlElement xmlNode in parameterMaps)
                {
                    var parameterMap = MapFactory.LoadParameterMap(xmlNode, SqlMapConfig);
                    sqlMap.ParameterMaps.Add(parameterMap);
                }
                #endregion

                #region Init Statement
                var statementNodes = xmlDoc.SelectNodes("//ns:Statement", xmlNsM);
                LoadStatementInSqlMap(sqlMap, statementNodes);

                var insertNodes = xmlDoc.SelectNodes("//ns:Insert", xmlNsM);
                LoadStatementInSqlMap(sqlMap, insertNodes);

                var updateNodes = xmlDoc.SelectNodes("//ns:Update", xmlNsM);
                LoadStatementInSqlMap(sqlMap, updateNodes);

                var deleteNodes = xmlDoc.SelectNodes("//ns:Delete", xmlNsM);
                LoadStatementInSqlMap(sqlMap, deleteNodes);

                var selectNodes = xmlDoc.SelectNodes("//ns:Select", xmlNsM);
                LoadStatementInSqlMap(sqlMap, selectNodes);
                #endregion

                return sqlMap;
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
