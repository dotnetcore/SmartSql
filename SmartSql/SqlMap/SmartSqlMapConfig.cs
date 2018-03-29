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
using Microsoft.Extensions.Logging;
using SmartSql.Abstractions.Logging;
namespace SmartSql.SqlMap
{
    [XmlRoot(Namespace = "http://SmartSql.net/schemas/SmartSqlMapConfig.xsd")]
    public class SmartSqlMapConfig
    {
        private ILogger<SmartSqlMapConfig> _logger = NullLoggerFactory.Instance.CreateLogger<SmartSqlMapConfig>();

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
                            _logger.LogDebug($"SmartSqlMapConfig. Path:{Path} Load MappedStatements Start!");
                            _mappedStatements = new Dictionary<string, Statement>();
                            foreach (var sqlmap in SmartSqlMaps)
                            {
                                foreach (var statement in sqlmap.Statements)
                                {
                                    var statementId = $"{sqlmap.Scope}.{statement.Id}";
                                    if (!_mappedStatements.ContainsKey(statementId))
                                    {
                                        _mappedStatements.Add(statementId, statement);
                                    }
                                    else
                                    {
                                        _logger.LogWarning($"SmartSqlMapConfig Load MappedStatements: StatementId:{statementId}  already exists!");
                                    }
                                }
                            }
                            _logger.LogDebug($"SmartSqlMapConfig. Path:{Path} Load MappedStatements End!");
                        }
                    }
                }
                return _mappedStatements;
            }
        }

        public void SetLogger(ILogger<SmartSqlMapConfig> logger)
        {
            this._logger = logger;
        }
        public void ResetMappedStatements()
        {
            lock (this)
            {
                _mappedStatements = null;
            }
        }
    }

    public class Settings
    {
        [XmlAttribute]
        public bool IsWatchConfigFile { get; set; }
        [XmlAttribute]
        public string ParameterPrefix { get; set; } = "$";
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
        public String TypeName { get { return Type.Split(',')[0].Trim(); } }
        [XmlIgnore]
        public String AssemblyName { get { return Type.Split(',')[1].Trim(); } }
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
                        if (_dbProviderFactory == null)
                        {
                            LoadFactory();
                        }
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
        [XmlAttribute]
        public ResourceType Type { get; set; } = ResourceType.File;
        public enum ResourceType
        {
            File = 1,
            Directory = 2,
        }
    }


}
