using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.SqlMap;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace SmartSql.Abstractions.Config
{
    public abstract class ConfigLoader : IConfigLoader
    {
        public abstract void Dispose();
        public abstract SmartSqlMapConfig Load(string path, ISmartSqlMapper smartSqlMapper);
        public SmartSqlMapConfig LoadConfig(ConfigStream configStream, ISmartSqlMapper smartSqlMapper)
        {
            using (configStream.Stream)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(SmartSqlMapConfig));
                SmartSqlMapConfig config = xmlSerializer.Deserialize(configStream.Stream) as SmartSqlMapConfig;
                config.Path = configStream.Path;
                config.SmartSqlMapper = smartSqlMapper;
                config.SmartSqlMaps = new List<SmartSqlMap> { };
                return config;
            }
        }
        public SmartSqlMap LoadSmartSqlMap(ConfigStream configStream, SmartSqlMapConfig smartSqlMapConfig)
        {
            using (configStream.Stream)
            {
                var sqlMap = new SmartSqlMap
                {
                    SmartSqlMapConfig = smartSqlMapConfig,
                    Path = configStream.Path,
                    Statements = new List<Statement> { },
                    Caches = new List<SqlMap.Cache> { }
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
                    var cache = SqlMap.Cache.Load(cacheNode);
                    sqlMap.Caches.Add(cache);
                }
                #endregion
                #region Init Statement
                var statementNodes = xmlDoc.SelectNodes("//ns:Statement", xmlNsM);
                foreach (XmlElement statementNode in statementNodes)
                {
                    var statement = Statement.Load(statementNode, sqlMap);
                    sqlMap.Statements.Add(statement);
                }
                #endregion
                
                return sqlMap;
            }
        }
    }

    public class ConfigStream
    {
        public String Path { get; set; }
        public Stream Stream { get; set; }
    }
}
