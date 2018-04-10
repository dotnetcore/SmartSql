using SmartSql.Abstractions;
using System.Linq;
using SmartSql.Common;
using SmartSql.Configuration.Statements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using SmartSql.Exceptions;
using System.Collections;

namespace SmartSql.Configuration
{
    [XmlRoot(Namespace = "http://SmartSql.net/schemas/SmartSqlMap.xsd")]
    public class SmartSqlMap
    {
        [XmlIgnore]
        public SmartSqlMapConfig SmartSqlMapConfig { get;  set; }
        [XmlIgnore]
        public String Path { get;  set; }
        [XmlAttribute]
        public String Scope { get; set; }
        public IList<Cache> Caches { get; set; }
        [XmlArray]
        public List<Statement> Statements { get; set; }
    }

}
