using SmartSql.Abstractions.DataSource;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using SmartSql.Common;
using System.Data.Common;
using SmartSql.Abstractions;
using System.Reflection;
using SmartSql.Abstractions.Logging;

namespace SmartSql.SqlMap
{
    [XmlRoot(Namespace = "http://SmartSql.net/schemas/SmartSqlMapConfig.xsd")]
    public class SmartSqlMapConfig
    {
        [XmlIgnore]
        private static readonly ILog _logger = LogManager.GetLogger(typeof(SmartSqlMapConfig));

        [XmlIgnore]
        public ISmartSqlMapper SmartSqlMapper { get; set; }
        [XmlIgnore]
        public String Path { get; set; }
        public Settings Settings { get; set; }
        public Database Database { get; set; }
        [XmlArray("SmartSqlMaps")]
        [XmlArrayItem("SmartSqlMap")]
        public List<SmartSqlMapSource> SmartSqlMapSources { get; set; }
        [XmlIgnore]
        public IList<SmartSqlMap> SmartSqlMaps { get; set; }
        private IDictionary<String, Statement> _mappedStatements;
        [XmlIgnore]
        public IDictionary<String, Statement> MappedStatements
        {
            get
            {
                if (_mappedStatements == null)
                {
                    lock (this)
                    {
                        if (_mappedStatements == null)
                        {
                            _logger.Debug($"SmartSqlMapConfig. Path:{Path} Load MappedStatements !");
                            _mappedStatements = new Dictionary<string, Statement>();
                            foreach (var sqlmap in SmartSqlMaps)
                            {
                                foreach (var statement in sqlmap.Statements)
                                {
                                    var statementId = $"{sqlmap.Scope}.{statement.Id}";
                                    _mappedStatements.Add(statementId, statement);
                                }
                            }
                        }
                    }
                }
                return _mappedStatements;
            }
        }

        public void ResetMappedStatements()
        {
            _mappedStatements = null;
        }
    }

    public class Settings
    {
        [XmlAttribute]
        public bool IsWatchConfigFile { get; set; }
    }

    public class Database
    {
        public DbProvider DbProvider { get; set; }
        [XmlElement("Write")]
        public WriteDataSource WriteDataSource { get; set; }
        [XmlElement("Read")]
        public List<ReadDataSource> ReadDataSources { get; set; }
    }

    public class DataSource : IDataSource
    {
        [XmlAttribute]
        public String Name { get; set; }
        [XmlAttribute]
        public String ConnectionString { get; set; }
    }

    public class WriteDataSource : IWriteDataSource
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string ConnectionString { get; set; }
    }

    public class ReadDataSource : IReadDataSource
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string ConnectionString { get; set; }
        [XmlAttribute]
        public int Weight { get; set; }
    }

    public class DbProvider
    {
        [XmlAttribute]
        public String Name { get; set; }
        [XmlAttribute]
        public String Type { get; set; }
        [XmlAttribute]
        public String ParameterPrefix { get; set; }
        [XmlIgnore]
        public String TypeName { get { return Type.Split(',')[0]; } }
        [XmlIgnore]
        public String AssemblyName { get { return Type.Split(',')[1]; } }
        private DbProviderFactory _dbProviderFactory;
        private void LoadFactory()
        {
            _dbProviderFactory = Assembly.Load(new AssemblyName { Name = AssemblyName })
                                         .GetType(TypeName)
                                         .GetField("Instance")
                                         .GetValue(null) as DbProviderFactory;
        }

        [XmlIgnore]
        public DbProviderFactory DbProviderFactory
        {
            get
            {
                if (_dbProviderFactory == null)
                {
                    lock (this)
                    {
                        LoadFactory();
                    }
                }
                return _dbProviderFactory;
            }
        }
    }

    public class SmartSqlMapSource
    {
        [XmlAttribute]
        public String Path { get; set; }
    }


}
