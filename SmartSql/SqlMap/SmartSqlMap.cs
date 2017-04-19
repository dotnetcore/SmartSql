using SmartSql.Abstractions;
using SmartSql.Abstractions.Logging;
using System.Linq;
using SmartSql.Common;
using SmartSql.SqlMap.Tags;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using SmartSql.Exceptions;
using System.Collections;

namespace SmartSql.SqlMap
{
    [XmlRoot(Namespace = "http://SmartSql.net/schemas/SmartSqlMap.xsd")]
    public class SmartSqlMap
    {
        [XmlIgnore]
        public SmartSqlMapConfig SmartSqlMapConfig { get; private set; }
        [XmlIgnore]
        public String FilePath { get; private set; }
        [XmlAttribute]
        public String Scope { get; set; }
        public IList<Cache> Caches { get; set; }
        [XmlArray]
        public List<Statement> Statements { get; set; }
        public static SmartSqlMap Load(String filePath, SmartSqlMapConfig smartSqlMapConfig)
        {
            var sqlMap = new SmartSqlMap
            {
                SmartSqlMapConfig = smartSqlMapConfig,
                FilePath = filePath,
                Statements = new List<Statement> { },
                Caches = new List<Cache> { }
            };
            using (var xmlFile = FileLoader.Load(filePath))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFile);
                XmlNamespaceManager xmlNsM = new XmlNamespaceManager(xmlDoc.NameTable);
                xmlNsM.AddNamespace("ns", "http://SmartSql.net/schemas/SmartSqlMap.xsd");
                sqlMap.Scope = xmlDoc.SelectSingleNode("//ns:SmartSqlMap", xmlNsM)
                    .Attributes["Scope"].Value;
                #region Init Caches
                var cacheNodes = xmlDoc.SelectNodes("//ns:Cache", xmlNsM);
                foreach (XmlElement cacheNode in cacheNodes)
                {
                    var cache = Cache.Load(cacheNode);
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

}
