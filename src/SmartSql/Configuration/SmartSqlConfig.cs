using SmartSql.DataSource;
using SmartSql.TypeHandlers;
using System;
using System.Collections.Generic;
using SmartSql.Configuration.Tags;
using SmartSql.Deserializer;
using SmartSql.Reflection.ObjectFactoryBuilder;
using SmartSql.Utils;
using Microsoft.Extensions.Logging;
using SmartSql.Exceptions;
using SmartSql.DbSession;
using SmartSql.Cache;
using SmartSql.IdGenerator;
using Microsoft.Extensions.Logging.Abstractions;
using SmartSql.AutoConverter;
using SmartSql.Command;
using SmartSql.Filters;

namespace SmartSql.Configuration
{
    public class SmartSqlConfig
    {
        public string Alias { get; set; }
        public Settings Settings { get; set; }
        public Database Database { get; set; }
        public Properties Properties { get; set; }
        public IDictionary<String, SqlMap> SqlMaps { get; set; }
        public ILoggerFactory LoggerFactory { get; set; }
        public IObjectFactoryBuilder ObjectFactoryBuilder { get; set; }
        public IDeserializerFactory DeserializerFactory { get; set; }
        public TypeHandlerFactory TypeHandlerFactory { get; set; }
        public ITagBuilderFactory TagBuilderFactory { get; set; }
        public StatementAnalyzer StatementAnalyzer { get; set; }
        public SqlParamAnalyzer SqlParamAnalyzer { get; set; }
        public SqlParamAnalyzer CacheTemplateAnalyzer { get; set; }
        public IMiddleware Pipeline { get; set; }
        public IDataSourceFilter DataSourceFilter { get; set; }
        public IDbSessionStore SessionStore { get; set; }
        public IDbSessionFactory DbSessionFactory { get; set; }
        public ICacheManager CacheManager { get; set; }
        public ICommandExecuter CommandExecuter { get; set; }
        public InvokeSucceedListener InvokeSucceedListener { get; set; }
        public IDictionary<String, IIdGenerator> IdGenerators { get; set; }
        public IDictionary<String, IAutoConverter> AutoConverters { get; set; }

        public IAutoConverter DefaultAutoConverter { get; set; }

        public FilterCollection Filters { get; set; }

        public SqlMap GetSqlMap(string scope)
        {
            if (!SqlMaps.TryGetValue(scope, out var sqlMap))
            {
                throw new SmartSqlException($"Can not find SqlMap.Scope:{scope}");
            }

            return sqlMap;
        }

        public Statement GetStatement(String fullId)
        {
            var scopeWithId = FullIdUtil.Parse(fullId);
            return GetSqlMap(scopeWithId.Item1).GetStatement(fullId);
        }

        public Cache GetCache(String fullId)
        {
            var scopeWithId = FullIdUtil.Parse(fullId);
            return GetSqlMap(scopeWithId.Item1).GetCache(fullId);
        }

        public ResultMap GetResultMap(string fullId)
        {
            var scopeWithId = FullIdUtil.Parse(fullId);
            return GetSqlMap(scopeWithId.Item1).GetResultMap(fullId);
        }

        public ParameterMap GetParameterMap(String fullId)
        {
            var scopeWithId = FullIdUtil.Parse(fullId);
            return GetSqlMap(scopeWithId.Item1).GetParameterMap(fullId);
        }

        public MultipleResultMap GetMultipleResultMap(String fullId)
        {
            var scopeWithId = FullIdUtil.Parse(fullId);
            return GetSqlMap(scopeWithId.Item1).GetMultipleResultMap(fullId);
        }

        public SmartSqlConfig()
        {
            Settings = Settings.Default;
            SqlMaps = new Dictionary<string, SqlMap>();
            Filters = new FilterCollection();
            ObjectFactoryBuilder = new ExpressionObjectFactoryBuilder();
            TagBuilderFactory = new TagBuilderFactory();
            TypeHandlerFactory = new TypeHandlerFactory();
            LoggerFactory = NullLoggerFactory.Instance;
            DeserializerFactory = new DeserializerFactory();
            Properties = new Properties();
            IdGenerators = new Dictionary<string, IIdGenerator>
            {
                {nameof(SnowflakeId.Default), SnowflakeId.Default}
            };
            AutoConverters = new Dictionary<string, IAutoConverter>();
            DbSessionFactory = new DbSessionFactory(this);
            SessionStore = new DbSessionStore(DbSessionFactory);
            StatementAnalyzer = new StatementAnalyzer();
            InvokeSucceedListener = new InvokeSucceedListener();
            DbSessionFactory.Opened += (sender, args) => { InvokeSucceedListener.BindDbSessionEvent(args.DbSession); };
            DefaultAutoConverter = NoneAutoConverter.INSTANCE;
        }
    }

    public class Settings
    {
        public static readonly Settings Default = new Settings
        {
            IgnoreParameterCase = false,
            IsCacheEnabled = false,
            ParameterPrefix = "$",
            EnablePropertyChangedTrack = false,
            IgnoreDbNull = false
        };

        public bool IgnoreParameterCase { get; set; }
        public bool IsCacheEnabled { get; set; }
        public string ParameterPrefix { get; set; }
        public bool EnablePropertyChangedTrack { get; set; }
        public bool IgnoreDbNull { get; set; }
    }
}