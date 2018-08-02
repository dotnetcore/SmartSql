using SmartSql.Configuration.Statements;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace SmartSql.Configuration.Maps
{
    [XmlRoot(Namespace = "http://SmartSql.net/schemas/SmartSqlMap.xsd")]
    public class SmartSqlMap
    {
        [XmlIgnore]
        public SmartSqlMapConfig SqlMapConfig { get; set; }
        [XmlIgnore]
        public String Path { get; set; }
        [XmlAttribute]
        public String Scope { get; set; }
        [XmlIgnore]
        public IList<Cache> Caches { get; set; }
        [XmlArray]
        public IList<Statement> Statements { get; set; }
        [XmlIgnore]
        public IList<ResultMap> ResultMaps { get; set; }
        [XmlIgnore]
        public IList<ParameterMap> ParameterMaps { get; set; }
        [XmlIgnore]
        public IList<MultipleResultMap>  MultipleResultMaps { get; set; }
    }



}
