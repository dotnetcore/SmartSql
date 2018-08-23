using SmartSql.Abstractions.DataSource;
using SmartSql.Abstractions.TypeHandler;
using SmartSql.Configuration.Maps;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Xml.Serialization;

namespace SmartSql.Configuration
{
    [XmlRoot(Namespace = "http://SmartSql.net/schemas/SmartSqlMapConfig.xsd")]
    public class SmartSqlMapConfig
    {
        [XmlIgnore]
        public String Path { get; set; }
        public Settings Settings { get; set; }
        public Database Database { get; set; }
        [XmlArray("SmartSqlMaps")]
        [XmlArrayItem("SmartSqlMap")]
        public List<SmartSqlMapSource> SmartSqlMapSources { get; set; }
        [XmlIgnore]
        public IList<SmartSqlMap> SmartSqlMaps { get; set; }
        [XmlArray("TypeHandlers")]
        public List<TypeHandler> TypeHandlers { get; set; }
    }

    public class Settings
    {
        [XmlAttribute]
        public bool IgnoreParameterCase { get; set; } = false;
        [XmlAttribute]
        public bool IsCacheEnabled { get; set; } = true;
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
        private DbProviderFactory _dbProviderFactory;
        [XmlIgnore]
        public DbProviderFactory Factory
        {
            get
            {
                if (_dbProviderFactory == null)
                {
                    _dbProviderFactory = DbProviderFactoryFactory.Create(Type);
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
            DirectoryWithAllSub = 3
        }
    }


    public class TypeHandler
    {
        [XmlAttribute]
        public String Name { get; set; }
        [XmlAttribute]
        public String Type { get; set; }
        [XmlIgnore]
        public ITypeHandler Handler { get; set; }
    }
}
