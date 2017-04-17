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
        [XmlArray]
        public List<Statement> Statements { get; set; }
        public static SmartSqlMap Load(String filePath, SmartSqlMapConfig smartSqlMapConfig)
        {
            var sqlMap = new SmartSqlMap
            {
                SmartSqlMapConfig = smartSqlMapConfig,
                FilePath = filePath,
                Statements = new List<Statement> { }
            };
            using (var xmlFile = FileLoader.Load(filePath))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFile);
                XmlNamespaceManager xmlNsM = new XmlNamespaceManager(xmlDoc.NameTable);
                xmlNsM.AddNamespace("ns", "http://SmartSql.net/schemas/SmartSqlMap.xsd");
                sqlMap.Scope = xmlDoc.SelectSingleNode("//ns:SmartSqlMap", xmlNsM)
                    .Attributes["Scope"].Value;
                var statementNodes = xmlDoc.SelectNodes("//ns:Statement", xmlNsM);
                foreach (XmlElement statementNode in statementNodes)
                {
                    var statement = Statement.Load(statementNode, sqlMap);
                    sqlMap.Statements.Add(statement);
                }
                return sqlMap;
            }

        }
    }

    public class Statement
    {
        [XmlIgnore]
        public SmartSqlMap SmartSqlMap { get; private set; }
        public static Statement Load(XmlElement statementNode, SmartSqlMap smartSqlMap)
        {
            var statement = new Statement
            {
                Id = statementNode.Attributes["Id"].Value,
                SqlTags = new List<ITag> { },
                SmartSqlMap = smartSqlMap
            };

            var tagNodes = statementNode.ChildNodes;
            foreach (XmlNode tagNode in tagNodes)
            {
                bool isIn = tagNode.Attributes?["In"] != null;
                var prepend = tagNode.Attributes?["Prepend"]?.Value;
                var property = tagNode.Attributes?["Property"]?.Value;
                var compareValue = tagNode.Attributes?["CompareValue"]?.Value;
                var bodyText = tagNode.InnerText.Replace("\n", "");
                #region Init Tag
                switch (tagNode.Name)
                {
                    case "#text":
                    case "#cdata-section":
                        {
                            statement.SqlTags.Add(new SqlText
                            {
                                BodyText = bodyText
                            });
                            break;
                        }
                    case "IsEmpty":
                        {
                            statement.SqlTags.Add(new IsEmpty
                            {
                                In = isIn,
                                Prepend = prepend,
                                Property = property,
                                BodyText = bodyText
                            });
                            break;
                        }
                    case "IsEqual":
                        {
                            statement.SqlTags.Add(new IsEqual
                            {
                                In = isIn,
                                Prepend = prepend,
                                Property = property,
                                CompareValue = compareValue,
                                BodyText = bodyText
                            });
                            break;
                        }
                    case "IsGreaterEqual":
                        {
                            statement.SqlTags.Add(new IsGreaterEqual
                            {
                                In = isIn,
                                Prepend = prepend,
                                Property = property,
                                CompareValue = compareValue,
                                BodyText = bodyText
                            });
                            break;
                        }
                    case "IsGreaterThan":
                        {
                            statement.SqlTags.Add(new IsGreaterThan
                            {
                                In = isIn,
                                Prepend = prepend,
                                Property = property,
                                CompareValue = compareValue,
                                BodyText = bodyText
                            });
                            break;
                        }
                    case "IsLessEqual":
                        {
                            statement.SqlTags.Add(new IsLessEqual
                            {
                                In = isIn,
                                Prepend = prepend,
                                Property = property,
                                CompareValue = compareValue,
                                BodyText = bodyText
                            });
                            break;
                        }
                    case "IsLessThan":
                        {
                            statement.SqlTags.Add(new IsLessThan
                            {
                                In = isIn,
                                Prepend = prepend,
                                Property = property,
                                CompareValue = compareValue,
                                BodyText = bodyText
                            });
                            break;
                        }
                    case "IsNotEmpty":
                        {
                            statement.SqlTags.Add(new IsNotEmpty
                            {
                                In = isIn,
                                Prepend = prepend,
                                Property = property,
                                BodyText = bodyText
                            });
                            break;
                        }
                    case "IsNotEqual":
                        {
                            statement.SqlTags.Add(new IsNotEqual
                            {
                                In = isIn,
                                Prepend = prepend,
                                Property = property,
                                CompareValue = compareValue,
                                BodyText = bodyText
                            });
                            break;
                        }
                    case "IsNotNull":
                        {
                            statement.SqlTags.Add(new IsNotNull
                            {
                                In = isIn,
                                Prepend = prepend,
                                Property = property,
                                BodyText = bodyText
                            });
                            break;
                        }
                    case "IsNull":
                        {
                            statement.SqlTags.Add(new IsNull
                            {
                                In = isIn,
                                Prepend = prepend,
                                Property = property,
                                BodyText = bodyText
                            });
                            break;
                        }
                    case "Include":
                        {
                            var refId = tagNode.Attributes?["RefId"]?.Value;
                            var refStatement = smartSqlMap.Statements.FirstOrDefault(m => m.Id == refId);
                            if (refStatement == null)
                            {
                                throw new SmartSqlException($"SmartSql.Statement.Load can not find statement.id:{refId}");
                            }
                            statement.SqlTags.Add(new Include
                            {
                                RefId = refId,
                                Ref = refStatement
                            });
                            break;
                        }
                    default: { break; };
                }
                #endregion
            }
            return statement;
        }

        [XmlAttribute]
        public String Id { get; set; }
        public List<ITag> SqlTags { get; set; }
        public String BuildSql(object paramObj)
        {
            String prefix = SmartSqlMap.SmartSqlMapConfig.Database.DbProvider.ParameterPrefix;
            StringBuilder sqlStrBuilder = new StringBuilder();
            foreach (ITag tag in SqlTags)
            {
                sqlStrBuilder.Append(tag.BuildSql(paramObj, prefix));
            }
            return sqlStrBuilder.ToString();
        }
    }




}
