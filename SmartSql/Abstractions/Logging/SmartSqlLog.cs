using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SmartSql.Abstractions.Logging
{
    [XmlRoot(Namespace = "http://SmartSql.net/schemas/SmartSqlLog.xsd")]
    public class SmartSqlLog
    {
        public LoggerAdapter LoggerAdapter { get; set; }
    }

    public class LoggerAdapter
    {
        [XmlAttribute]
        public String Name { get; set; }
        [XmlAttribute]
        public String Type { get; set; }
        [XmlIgnore]
        public String TypeName { get { return Type.Split(',')[0]; } }
        [XmlIgnore]
        public String AssemblyName { get { return Type.Split(',')[1]; } }
    }
}
