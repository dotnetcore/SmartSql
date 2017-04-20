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
        public ISmartSqlMapper SmartSqlMapper { get; private set; }
        [XmlIgnore]
        public String FilePath { get; private set; }
        public static SmartSqlMapConfig Load(String filePath, ISmartSqlMapper smartSqlMapper)
        {
            _logger.Debug($"SmartSql.SmartSqlMapConfig Load: {filePath} Starting");
            var config = LoadConfig(filePath, smartSqlMapper);
            _logger.Debug($"SmartSql.SmartSqlMapConfig Load: {filePath} End");
            smartSqlMapper.LoadConfig(config);
            if (config.Settings.IsWatchConfigFile)
            {
                _logger.Debug($"SmartSql.SmartSqlMapConfig.Load Add WatchConfig: {filePath} .");
                WatchConfig(smartSqlMapper);
            }
            return config;
        }
        /// <summary>
        /// 监控配置文件-热更新
        /// </summary>
        /// <param name="smartSqlMapper"></param>
        /// <param name="config"></param>
        private static void WatchConfig(ISmartSqlMapper smartSqlMapper)
        {
            var config = smartSqlMapper.SqlMapConfig;
            #region SmartSqlMapConfig File Watch
            var cofigFileInfo = FileLoader.GetInfo(config.FilePath);
            FileWatcherLoader.Instance.Watch(cofigFileInfo, () =>
            {
                _logger.Debug($"SmartSql.SmartSqlMapConfig Changed ReloadConfig: {config.FilePath} Starting");
                var newConfig = LoadConfig(config.FilePath, smartSqlMapper);
                smartSqlMapper.LoadConfig(newConfig);
                _logger.Debug($"SmartSql.SmartSqlMapConfig Changed ReloadConfig: {config.FilePath} End");
            });
            #endregion
            #region SmartSqlMaps File Watch
            foreach (var sqlmap in config.SmartSqlMaps)
            {
                #region SqlMap File Watch
                var sqlMapFileInfo = FileLoader.GetInfo(sqlmap.FilePath);
                FileWatcherLoader.Instance.Watch(sqlMapFileInfo, () =>
                {
                    _logger.Debug($"SmartSql.SmartSqlMapConfig Changed Reload SmartSqlMap: {sqlmap.FilePath} Starting");
                    var newSqlmap = SmartSqlMap.Load(sqlmap.FilePath, config);
                    sqlmap.Scope = newSqlmap.Scope;
                    sqlmap.Statements = newSqlmap.Statements;
                    _logger.Debug($"SmartSql.SmartSqlMapConfig Changed Reload SmartSqlMap: {sqlmap.FilePath} End");
                });
                #endregion
            }
            #endregion
        }

        private static SmartSqlMapConfig LoadConfig(String filePath, ISmartSqlMapper smartSqlMapper)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SmartSqlMapConfig));
            SmartSqlMapConfig config = null;
            using (var configFile = FileLoader.Load(filePath))
            {
                config = xmlSerializer.Deserialize(configFile) as SmartSqlMapConfig;
                config.FilePath = filePath;
                config.SmartSqlMapper = smartSqlMapper;
            }
            config.SmartSqlMaps = new List<SmartSqlMap> { };
            foreach (var sqlmapSource in config.SmartSqlMapSources)
            {
                var sqlmap = SmartSqlMap.Load(sqlmapSource.Path, config);
                config.SmartSqlMaps.Add(sqlmap);
            }
            return config;
        }

        public Settings Settings { get; set; }
        public Database Database { get; set; }
        [XmlArray("SmartSqlMaps")]
        [XmlArrayItem("SmartSqlMap")]
        public List<SmartSqlMapSource> SmartSqlMapSources { get; set; }
        [XmlIgnore]
        public IList<SmartSqlMap> SmartSqlMaps { get; private set; }
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
                            _logger.Debug($"SmartSqlMapConfig. FilePath:{FilePath} Load MappedStatements !");
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
