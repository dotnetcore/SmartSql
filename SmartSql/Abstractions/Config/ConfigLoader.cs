using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Xml;
using System.Xml.Serialization;
using SmartSql.Configuration;
using SmartSql.Configuration.Statements;

namespace SmartSql.Abstractions.Config
{
    public abstract class ConfigLoader : IConfigLoader
    {
        private StatementFactory statementFactory = new StatementFactory();
        public abstract Action<ConfigChangedEvent> OnChanged { get; set; }
        public abstract SmartSqlMapConfig SqlMapConfig { get; protected set; }

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
                return SqlMapConfig;
            }
        }
        public SmartSqlMap LoadSmartSqlMap(ConfigStream configStream)
        {
            using (configStream.Stream)
            {
                var sqlMap = new SmartSqlMap
                {
                    SmartSqlMapConfig = SqlMapConfig,
                    Path = configStream.Path,
                    Statements = new List<Statement> { },
                    Caches = new List<Configuration.Cache> { }
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
                    var cache = Configuration.Cache.Load(cacheNode);
                    sqlMap.Caches.Add(cache);
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
                var statement = statementFactory.Load(statementNode, sqlMap);
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
