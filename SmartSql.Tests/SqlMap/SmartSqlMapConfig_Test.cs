using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.SqlMap;
using Xunit;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace SmartSql.Tests.SqlMap
{
    public class SmartSqlMapConfig_Test
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
        public void SmartSqlMap_Serialize()
        {
            //XmlSerializer serializer = new XmlSerializer(typeof(SmartSqlMap));
            //var xmlFile = File.Create(@"E:\SmartSqlMap.xml");
            //serializer.Serialize(xmlFile, new SmartSqlMap
            //{
            //    Scope = "T_Test",
            //    Statements = new List<Statement> {
            //          new Statement{
            //               Id="GetList",
            //                SqlTags=new List<SqlTag>{
            //                    new SqlTag{Type= TagType.SqlText, BodyText=new string[]{"Select * From T_Test Where 1=1"}},
            //                    new SqlTag{Type= TagType.IsNotEmpty, Prepend="And", Property="Id", BodyText=new string[]{"Id=@Id" } },
            //                    new SqlTag{Type= TagType.IsNotEmpty, Prepend="And", Property="Ids", Iterate=new Iterate{
            //                         Prepend="Ids In", Open="(", Close=")", Conjunction=",", Property="Ids", Type= TagType.Iterate, BodyText=new string[]{"#Ids[]#" }
            //                    } },
            //                }
            //          }
            //      }
            //});
            //xmlFile.Dispose();
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
