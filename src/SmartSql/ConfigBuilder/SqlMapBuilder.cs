using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Xml;
using SmartSql.AutoConverter;
using SmartSql.Cache;
using SmartSql.Cache.Default;
using SmartSql.Configuration;
using SmartSql.Configuration.Tags;
using SmartSql.DataSource;
using SmartSql.Exceptions;
using SmartSql.Reflection;
using SmartSql.Reflection.TypeConstants;

namespace SmartSql.ConfigBuilder
{
    public class SqlMapBuilder
    {
        public const String SMART_SQL_MAP_NAMESPACE = "http://SmartSql.net/schemas/SmartSqlMap.xsd";
        public const String SQLMAP_PREFIX = "SqlMap";
        public const String TYPE_ATTRIBUTE = "Type";
        protected SmartSqlConfig SmartSqlConfig { get; }
        protected XmlDocument XmlSqlMap { get; }
        protected XmlNode XmlSqlMapRoot { get; }
        protected XmlNamespaceManager XmlNsManager { get; }
        protected SqlMap SqlMap { get; private set; }

        public SqlMapBuilder(XmlDocument xmlSqlMap, SmartSqlConfig smartSqlConfig)
        {
            XmlSqlMap = xmlSqlMap;
            SmartSqlConfig = smartSqlConfig;
            XmlNsManager = new XmlNamespaceManager(xmlSqlMap.NameTable);
            XmlNsManager.AddNamespace(SQLMAP_PREFIX, SMART_SQL_MAP_NAMESPACE);
            XmlSqlMapRoot = xmlSqlMap.SelectSingleNode($"/{SQLMAP_PREFIX}:SmartSqlMap", XmlNsManager);
        }

        public SqlMap Build()
        {
            SqlMap = new SqlMap
            {
                SmartSqlConfig = SmartSqlConfig,
                Path = XmlSqlMap.BaseURI,
                Statements = new Dictionary<String, Statement> { },
                Caches = new Dictionary<String, Configuration.Cache> { },
                ParameterMaps = new Dictionary<String, ParameterMap> { },
                ResultMaps = new Dictionary<String, ResultMap> { },
                MultipleResultMaps = new Dictionary<String, MultipleResultMap> { }
            };
            if (!XmlSqlMapRoot.Attributes.TryGetValueAsString(nameof(SqlMap.Scope), out string scope,
                SmartSqlConfig.Properties))
            {
                throw new SmartSqlException("SqlMap.Code can not be null.");
            }

            var sqlMap = SqlMap;
            if (SmartSqlConfig.SqlMaps.ContainsKey(scope))
            {
                SmartSqlConfig.SqlMaps.TryGetValue(scope, out sqlMap);
                SmartSqlConfig.SqlMaps.Remove(scope);
            }

            SqlMap = sqlMap;

            SqlMap.Scope = scope;

            BuildCaches();
            BuildParameterMaps();
            BuildResultMaps();
            BuildMultipleResultMaps();
            BuildStatements();
            BuildAutoConverter();
            SmartSqlConfig.SqlMaps.Add(SqlMap.Scope, SqlMap);
            return SqlMap;
        }

        #region Caches

        private void BuildCaches()
        {
            var cacheNodes = XmlSqlMapRoot.SelectNodes($"{SQLMAP_PREFIX}:Caches/{SQLMAP_PREFIX}:Cache", XmlNsManager);
            if (cacheNodes != null)
                foreach (XmlElement cacheNode in cacheNodes)
                {
                    BuildCache(cacheNode);
                }
        }

        private void BuildCache(XmlElement cacheNode)
        {
            cacheNode.Attributes.TryGetValueAsString(nameof(Configuration.Cache.Id), out var id,
                SmartSqlConfig.Properties);
            cacheNode.Attributes.TryGetValueAsString(nameof(Configuration.Cache.Type), out var type,
                SmartSqlConfig.Properties);

            var cache = new Configuration.Cache
            {
                Id = id,
                Type = type,
                Parameters = new Dictionary<String, Object>(),
                FlushOnExecutes = new List<FlushOnExecute>()
            };
            if (cache.Id.IndexOf('.') < 0)
            {
                cache.Id = $"{SqlMap.Scope}.{cache.Id}";
            }

            foreach (XmlNode childNode in cacheNode.ChildNodes)
            {
                switch (childNode.Name)
                {
                    case "Property":
                    {
                        childNode.Attributes.TryGetValueAsString("Name", out var name, SmartSqlConfig.Properties);
                        childNode.Attributes.TryGetValueAsString("Value", out var propVal, SmartSqlConfig.Properties);
                        cache.Parameters.Add(name, propVal);
                        break;
                    }

                    case "FlushInterval":
                    {
                        childNode.Attributes.TryGetValueAsInt32("Hours", out var hours, SmartSqlConfig.Properties);
                        childNode.Attributes.TryGetValueAsInt32("Minutes", out var minutes, SmartSqlConfig.Properties);
                        childNode.Attributes.TryGetValueAsInt32("Seconds", out var seconds, SmartSqlConfig.Properties);
                        cache.FlushInterval = new FlushInterval
                        {
                            Hours = hours,
                            Minutes = minutes,
                            Seconds = seconds
                        };

                        cache.Parameters.Add("FlushInterval", cache.FlushInterval);
                        break;
                    }

                    case "FlushOnExecute":
                    {
                        childNode.Attributes.TryGetValueAsString("Statement", out var statementId,
                            SmartSqlConfig.Properties);
                        if (!String.IsNullOrEmpty(statementId))
                        {
                            if (statementId.IndexOf('.') < 0)
                            {
                                statementId = $"{SqlMap.Scope}.{statementId}";
                            }

                            cache.FlushOnExecutes.Add(new FlushOnExecute
                            {
                                Statement = statementId
                            });
                        }

                        break;
                    }
                }
            }

            cache.Parameters.Add("Cache.Id", cache.Id);

            cache.Provider = CacheProviderUtil.Create(cache);
            SqlMap.Caches.Add(cache.Id, cache);
        }

        #endregion

        #region  ParameterMaps

        private void BuildParameterMaps()
        {
            var parameterMapsNodes =
                XmlSqlMapRoot.SelectNodes($"//{SQLMAP_PREFIX}:ParameterMaps/{SQLMAP_PREFIX}:ParameterMap",
                    XmlNsManager);
            if (parameterMapsNodes != null)
                foreach (XmlElement xmlNode in parameterMapsNodes)
                {
                    BuildParameterMap(xmlNode);
                }
        }

        private void BuildParameterMap(XmlElement xmlNode)
        {
            xmlNode.Attributes.TryGetValueAsString(nameof(ParameterMap.Id), out var id, SmartSqlConfig.Properties);
            ParameterMap parameterMap = new ParameterMap
            {
                Id = id,
                Parameters = new Dictionary<string, Parameter>()
            };
            if (parameterMap.Id.IndexOf('.') < 0)
            {
                parameterMap.Id = $"{SqlMap.Scope}.{parameterMap.Id}";
            }

            BuildParameters(xmlNode, parameterMap);
            SqlMap.ParameterMaps.Add(parameterMap.Id, parameterMap);
        }

        private void BuildParameters(XmlElement xmlNode, ParameterMap parameterMap)
        {
            var parameterNodes = xmlNode.SelectNodes($"./{SQLMAP_PREFIX}:Parameter", XmlNsManager);
            if (parameterNodes != null)
                foreach (XmlNode parameterNode in parameterNodes)
                {
                    parameterNode.Attributes.TryGetValueAsString("Property", out var property,
                        SmartSqlConfig.Properties);
                    var parameter = new Parameter
                    {
                        Property = property
                    };
                    if (parameterNode.Attributes.TryGetValueAsString("TypeHandler", out var handlerName,
                        SmartSqlConfig.Properties))
                    {
                        parameter.Handler = SmartSqlConfig.TypeHandlerFactory.GetTypeHandler(handlerName);
                    }

                    parameterMap.Parameters.Add(parameter.Property, parameter);
                }
        }

        #endregion

        #region  ResultMaps

        private void BuildResultMaps()
        {
            var resultMapsNodes =
                XmlSqlMapRoot.SelectNodes($"//{SQLMAP_PREFIX}:ResultMaps/{SQLMAP_PREFIX}:ResultMap", XmlNsManager);
            if (resultMapsNodes != null)
                foreach (XmlElement xmlNode in resultMapsNodes)
                {
                    BuildResultMap(xmlNode);
                }
        }

        private void BuildResultMap(XmlElement xmlNode)
        {
            xmlNode.Attributes.TryGetValueAsString(nameof(ResultMap.Id), out var id, SmartSqlConfig.Properties);
            ResultMap resultMap = new ResultMap
            {
                Id = id,
                Properties = new Dictionary<string, Property>()
            };
            if (resultMap.Id.IndexOf('.') < 0)
            {
                resultMap.Id = $"{SqlMap.Scope}.{resultMap.Id}";
            }

            BuildResultCtor(xmlNode, resultMap);
            BuildResultProperty(xmlNode, resultMap);
            SqlMap.ResultMaps.Add(resultMap.Id, resultMap);
        }

        private void BuildResultCtor(XmlElement xmlNode, ResultMap resultMap)
        {
            var ctorNode = xmlNode.SelectSingleNode($"./{SQLMAP_PREFIX}:Constructor", XmlNsManager);
            var argNodes = ctorNode?.SelectNodes($"./{SQLMAP_PREFIX}:Arg", XmlNsManager);
            if (argNodes == null || argNodes.Count <= 0) return;
            var ctorMap = new Constructor
            {
                Args = new List<Arg>()
            };
            foreach (XmlNode argNode in argNodes)
            {
                argNode.Attributes.TryGetValueAsString(nameof(Arg.Column), out var column, SmartSqlConfig.Properties);
                argNode.Attributes.TryGetValueAsString(nameof(Arg.CSharpType), out var argTypeStr,
                    SmartSqlConfig.Properties);
                var arg = new Arg {Column = column, CSharpType = ArgTypeConvert(argTypeStr)};
                ctorMap.Args.Add(arg);
            }

            resultMap.Constructor = ctorMap;
        }

        private static Type ArgTypeConvert(string typeStr)
        {
            switch (typeStr)
            {
                case "Boolean":
                {
                    return typeof(Boolean);
                }

                case "Char":
                {
                    return typeof(Char);
                }

                case "SByte":
                {
                    return typeof(SByte);
                }

                case "Byte":
                {
                    return typeof(Byte);
                }

                case "Int16":
                {
                    return typeof(Int16);
                }

                case "UInt16":
                {
                    return typeof(UInt16);
                }

                case "Int32":
                {
                    return typeof(Int32);
                }

                case "UInt32":
                {
                    return typeof(UInt32);
                }

                case "Int64":
                {
                    return typeof(Int64);
                }

                case "UInt64":
                {
                    return typeof(UInt64);
                }

                case "Single":
                {
                    return typeof(Single);
                }

                case "Double":
                {
                    return typeof(Double);
                }

                case "Decimal":
                {
                    return typeof(Decimal);
                }

                case "DateTime":
                {
                    return typeof(DateTime);
                }

                case "String":
                {
                    return typeof(String);
                }

                case "Guid":
                {
                    return typeof(Guid);
                }

                default:
                {
                    return Type.GetType(typeStr, true);
                }
            }
        }

        private void BuildResultProperty(XmlElement xmlNode, ResultMap resultMap)
        {
            var resultNodes = xmlNode.SelectNodes($"./{SQLMAP_PREFIX}:Result", XmlNsManager);
            if (resultNodes == null) return;
            foreach (XmlNode resultNode in resultNodes)
            {
                resultNode.Attributes.TryGetValueAsString("Property", out var name, SmartSqlConfig.Properties);
                if (!resultNode.Attributes.TryGetValueAsString("Column", out var column, SmartSqlConfig.Properties))
                {
                    column = name;
                }

                var property = new Property
                {
                    Name = name,
                    Column = column
                };
                if (resultNode.Attributes.TryGetValueAsString("TypeHandler", out var handlerName,
                    SmartSqlConfig.Properties))
                {
                    property.TypeHandler = handlerName;
                    SmartSqlConfig.TypeHandlerFactory.GetTypeHandler(handlerName); //Check Named TypeHandler
                }

                resultMap.Properties.Add(property.Column, property);
            }
        }

        #endregion

        #region  MultipleResultMaps

        private void BuildMultipleResultMaps()
        {
            var multipleResultMapsNode =
                XmlSqlMapRoot.SelectNodes($"{SQLMAP_PREFIX}:MultipleResultMaps/{SQLMAP_PREFIX}:MultipleResultMap",
                    XmlNsManager);
            if (multipleResultMapsNode == null) return;
            foreach (XmlElement xmlNode in multipleResultMapsNode)
            {
                BuildMultipleResultMap(xmlNode);
            }
        }

        private void BuildMultipleResultMap(XmlNode mulResultNode)
        {
            mulResultNode.Attributes.TryGetValueAsString(nameof(MultipleResultMap.Id), out var id,
                SmartSqlConfig.Properties);
            var multipleResultMap = new MultipleResultMap
            {
                Id = id,
                Results = new List<Result> { }
            };
            if (multipleResultMap.Id.IndexOf('.') < 0)
            {
                multipleResultMap.Id = $"{SqlMap.Scope}.{multipleResultMap.Id}";
            }

            foreach (XmlNode childNode in mulResultNode.ChildNodes)
            {
                childNode.Attributes.TryGetValueAsString("Property", out var property, SmartSqlConfig.Properties);
                childNode.Attributes.TryGetValueAsString("MapId", out var mapId, SmartSqlConfig.Properties);
                var result = new Result
                {
                    Property = property,
                    MapId = mapId,
                };
                if (result.MapId?.IndexOf('.') < 0)
                {
                    result.MapId = $"{SqlMap.Scope}.{result.MapId}";
                }

                if (childNode.Name == "Root")
                {
                    result.Property = Result.ROOT_PROPERTY;
                    multipleResultMap.Root = result;
                    multipleResultMap.Results.Add(result);
                }
                else
                {
                    multipleResultMap.Results.Add(result);
                }
            }

            SqlMap.MultipleResultMaps.Add(multipleResultMap.Id, multipleResultMap);
        }

        #endregion

        #region  Statements

        private void BuildStatements()
        {
            var statementNodes =
                XmlSqlMapRoot.SelectNodes($"{SQLMAP_PREFIX}:Statements/{SQLMAP_PREFIX}:Statement", XmlNsManager);
            if (statementNodes == null) return;
            foreach (XmlElement statementNode in statementNodes)
            {
                BuildStatement(statementNode);
            }
        }

        private void BuildStatement(XmlNode statementNode)
        {
            statementNode.Attributes.TryGetValueAsString(nameof(Statement.Id), out var id, SmartSqlConfig.Properties);
            statementNode.Attributes.TryGetValueAsString(nameof(Statement.ReadDb), out var readDb,
                SmartSqlConfig.Properties);
            statementNode.Attributes.TryGetValueAsString(nameof(Statement.Cache), out var cacheId,
                SmartSqlConfig.Properties);
            statementNode.Attributes.TryGetValueAsString(nameof(Statement.ParameterMap), out var parameterId,
                SmartSqlConfig.Properties);
            statementNode.Attributes.TryGetValueAsString(nameof(Statement.ResultMap), out var resultMapId,
                SmartSqlConfig.Properties);
            statementNode.Attributes.TryGetValueAsString(nameof(Statement.MultipleResultMap),
                out var multipleResultMapId, SmartSqlConfig.Properties);
            if (!statementNode.Attributes.TryGetValueAsBoolean(nameof(Statement.EnablePropertyChangedTrack),
                out var enablePropertyChangedTrack, SmartSqlConfig.Properties))
            {
                if (SmartSqlConfig.Settings.EnablePropertyChangedTrack &&
                    (
                        id.Equals("GetEntity", StringComparison.OrdinalIgnoreCase)
                        ||
                        id.Equals("Update", StringComparison.OrdinalIgnoreCase)
                    ))
                {
                    enablePropertyChangedTrack = true;
                }
            }


            int? commandTimeout = null;
            if (statementNode.Attributes.TryGetValueAsInt32(nameof(Statement.CommandTimeout), out var cmdTimeout,
                SmartSqlConfig.Properties))
            {
                commandTimeout = cmdTimeout;
            }

            var statement = new Statement
            {
                Id = id,
                SqlTags = new List<ITag> { },
                ReadDb = readDb,
                EnablePropertyChangedTrack = enablePropertyChangedTrack,
                SqlMap = SqlMap,
                CacheId = cacheId,
                ParameterMapId = parameterId,
                ResultMapId = resultMapId,
                CommandTimeout = commandTimeout,
                MultipleResultMapId = multipleResultMapId,
                IncludeDependencies = new List<Include>()
            };

            if (!String.IsNullOrEmpty(statement.ReadDb))
            {
                if (!SmartSqlConfig.Database.Reads.ContainsKey(statement.ReadDb))
                {
                    throw new SmartSqlException(
                        $"Statement.Id:{statement.FullSqlId} can not find ReadDb:{statement.ReadDb}!");
                }
            }

            #region AutoConverter

            if (statementNode.Attributes.TryGetValueAsString("AutoConverter", out String autoConverterName,
                SmartSqlConfig.Properties))
            {
                if (!SmartSqlConfig.AutoConverters.TryGetValue(autoConverterName, out var autoConverter))
                {
                    throw new SmartSqlException(
                        $"Statement.Id:{statement.FullSqlId} can not find AutoConverter:{autoConverterName}!");
                }

                statement.AutoConverter = autoConverter;
            }

            #endregion

            #region Init CacheId & ResultMapId & ParameterMapId & MultipleResultMapId

            if (statement.CacheId?.IndexOf('.') < 0)
            {
                statement.CacheId = $"{SqlMap.Scope}.{statement.CacheId}";
            }

            if (statement.ParameterMapId?.IndexOf('.') < 0)
            {
                statement.ParameterMapId = $"{SqlMap.Scope}.{statement.ParameterMapId}";
            }

            if (statement.ResultMapId?.IndexOf('.') < 0)
            {
                statement.ResultMapId = $"{SqlMap.Scope}.{statement.ResultMapId}";
            }

            if (statement.MultipleResultMapId?.IndexOf('.') < 0)
            {
                statement.MultipleResultMapId = $"{SqlMap.Scope}.{statement.MultipleResultMapId}";
            }

            #endregion

            #region Init CommandType & SourceChoice & Transaction

            statementNode.Attributes.TryGetValueAsString(nameof(Statement.CommandType), out var cmdTypeStr,
                SmartSqlConfig.Properties);
            statementNode.Attributes.TryGetValueAsString(nameof(Statement.SourceChoice), out var sourceChoiceStr,
                SmartSqlConfig.Properties);
            statementNode.Attributes.TryGetValueAsString(nameof(Statement.Transaction), out var transactionStr,
                SmartSqlConfig.Properties);
            if (Enum.TryParse<CommandType>(cmdTypeStr, out CommandType cmdType))
            {
                statement.CommandType = cmdType;
            }

            if (Enum.TryParse<DataSourceChoice>(sourceChoiceStr, out DataSourceChoice sourceChoice))
            {
                statement.SourceChoice = sourceChoice;
            }

            if (Enum.TryParse<IsolationLevel>(transactionStr, out IsolationLevel isolationLevel))
            {
                statement.Transaction = isolationLevel;
            }

            #endregion

            var tagNodes = statementNode.ChildNodes;
            foreach (XmlNode tagNode in tagNodes)
            {
                var tag = LoadTag(tagNode, statement);
                if (tag != null)
                {
                    statement.SqlTags.Add(tag);
                }
            }

            SqlMap.Statements.Add(statement.FullSqlId, statement);
        }


        private ITag LoadTag(XmlNode xmlNode, Statement statement)
        {
            ITag tag = null;

            #region Init Tag

            switch (xmlNode.Name)
            {
                case "#comment":
                {
                    return null;
                }

                case "#text":
                case "#cdata-section":
                {
                    return SmartSqlConfig.TagBuilderFactory.Get(xmlNode.Name).Build(xmlNode, statement);
                }

                default:
                {
                    tag = SmartSqlConfig.TagBuilderFactory.Get(xmlNode.Name).Build(xmlNode, statement);
                    break;
                }
            }

            #endregion

            foreach (XmlNode childNode in xmlNode)
            {
                ITag childTag = LoadTag(childNode, statement);
                if (childTag == null || tag == null) continue;
                childTag.Parent = tag;
                (tag as Tag).ChildTags.Add(childTag);
            }

            return tag;
        }

        #endregion

        #region Use AutoConverter

        private void BuildAutoConverter()
        {
            var useAutoConverterNode =
                XmlSqlMapRoot.SelectSingleNode($"{SQLMAP_PREFIX}:UseAutoConverter", XmlNsManager);
            if (useAutoConverterNode == null)
            {
                SqlMap.AutoConverter = SmartSqlConfig.DefaultAutoConverter;
                return;
            }

            if (useAutoConverterNode.Attributes.TryGetValueAsBoolean("Disabled", out bool disabled,
                SmartSqlConfig.Properties))
            {
                if (disabled)
                {
                    SqlMap.AutoConverter = NoneAutoConverter.INSTANCE;
                    return;
                }
            }

            if (!useAutoConverterNode.Attributes.TryGetValueAsString("Name", out String autoConverterName,
                SmartSqlConfig.Properties))
            {
                throw new SmartSqlException(
                    $"Scope:{SqlMap.Scope} UseAutoConverter.Name can not be null");
            }

            if (!SmartSqlConfig.AutoConverters.TryGetValue(autoConverterName, out var autoConverter))
            {
                throw new SmartSqlException($"Scope:{SqlMap.Scope} can not find AutoConverter:[{autoConverterName}]");
            }

            SqlMap.AutoConverter = autoConverter;
        }

        #endregion
    }
}