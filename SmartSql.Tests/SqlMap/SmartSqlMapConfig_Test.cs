using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.SqlMap;
using Xunit;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using SmartSql.Abstractions;
using System.Threading;

namespace SmartSql.Tests.SqlMap
{
    public class SmartSqlMapConfig_Test : TestBase
    {
        [Fact]
        public void Load()
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, "Maps", "SmartSqlMapConfig.xml");
            var config = SmartSqlMapConfig.Load("Maps/SmartSqlMapConfig.xml", null);
            Assert.NotNull(config);
        }
        [Fact]
        public void Serialize()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SmartSqlMapConfig));
            var xmlFile = File.Create(@"E:\SmartSqlMapConfig.xml");
            serializer.Serialize(xmlFile, new SmartSqlMapConfig
            {
                Database = new Database
                {
                    DbProvider = new DbProvider { },
                    WriteDataSource = new WriteDataSource { },
                    ReadDataSources = new List<ReadDataSource> {
                             new ReadDataSource{ },
                             new ReadDataSource{ },
                         }
                },
                SmartSqlMapSources = new List<SmartSqlMapSource> {
                      new SmartSqlMapSource{ },
                      new SmartSqlMapSource{ },
                      new SmartSqlMapSource{ },
                 }
            });
            xmlFile.Dispose();
        }

        [Fact]
        public void Query_OnChangeConfig()
        {
            int i = 0;
            for (i = 0; i < 10; i++)
            {
                var list = SqlMapper.Query<T_Test>(new RequestContext
                {
                    Scope = "T_Test",
                    SqlId = "GetList",
                    Request = new { Ids = new long[] { 1, 2, 3, 4 } }
                });
                Thread.Sleep(5000);
            }
        }

        [Fact]
        public void SmartSqlMap_Load()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SmartSqlMap));
            var xmlFile = File.OpenRead(@"E:\SmartSqlMap.xml");
            var smartSqlMap = serializer.Deserialize(xmlFile);
            xmlFile.Dispose();
            Assert.NotNull(smartSqlMap);
        }

        [Fact]
        public void StatementLoad()
        {
            string filePath = @"E:\SmartSql\SmartSql\SqlMap\SmartSqlMap.xml";
            var xmlFile = File.OpenRead(filePath);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFile);
            var scope = xmlDoc.SelectSingleNode("//SmartSqlMap").Attributes["Scope"];

            var statementNodes = xmlDoc.SelectNodes("//Statement");
            foreach (XmlElement statementNode in statementNodes)
            {
                var tagNodes = statementNode.ChildNodes;
                foreach (XmlNode tagNode in tagNodes)
                {

                }
            }
            xmlFile.Dispose();

            Assert.NotNull(statementNodes);
        }
    }
}
