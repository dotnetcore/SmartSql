using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.Extensions.Logging;
using SmartSql.Annotations;
using SmartSql.Cache;
using SmartSql.Configuration;
using SmartSql.Configuration.Tags;
using SmartSql.DataSource;
using SmartSql.DyRepository.Annotations;
using SmartSql.Exceptions;
using SmartSql.Reflection.TypeConstants;
using SmartSql.TypeHandlers;
using SmartSql.Utils;

namespace SmartSql.DyRepository
{
    public class EmitRepositoryBuilder : IRepositoryBuilder
    {
        private readonly ScopeTemplateParser _templateParser;
        private AssemblyBuilder _assemblyBuilder;
        private ModuleBuilder _moduleBuilder;
        private readonly Func<Type, MethodInfo, String> _sqlIdNamingConvert;
        private readonly ILogger _logger;
        private readonly StatementAnalyzer _statementAnalyzer;

        public EmitRepositoryBuilder(
            string scopeTemplate
            , Func<Type, MethodInfo, String> sqlIdNamingConvert
            , ILogger logger
        )
        {
            _sqlIdNamingConvert = sqlIdNamingConvert;
            _logger = logger;
            _templateParser = new ScopeTemplateParser(scopeTemplate);
            _statementAnalyzer = new StatementAnalyzer();
            InitAssembly();
        }

        private void InitAssembly()
        {
            string assemblyName = "SmartSql.RepositoryImpl" + this.GetHashCode();
            _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName
            {
                Name = assemblyName
            }, AssemblyBuilderAccess.Run);
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule(assemblyName + ".dll");
        }

        private void EmitBuildCtor(string scope, TypeBuilder typeBuilder, FieldBuilder sqlMapperField,
            FieldBuilder scopeField)
        {
            var paramTypes = new Type[] {ISqlMapperType.Type};
            var ctorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public, CallingConventions.Standard, paramTypes);
            var ilGen = ctorBuilder.GetILGenerator();
            ilGen.LoadArg(0);
            ilGen.Call(CommonType.Object.GetConstructor(Type.EmptyTypes));
            ilGen.LoadArg(0);
            ilGen.LoadArg(1);
            ilGen.FieldSet(sqlMapperField);
            ilGen.LoadArg(0);
            ilGen.LoadString(scope);
            ilGen.FieldSet(scopeField);
            ilGen.Return();
        }

        private void BuildInternalGet(TypeBuilder typeBuilder, MethodInfo methodInfo, FieldBuilder sqlMapperField)
        {
            var implMethod = typeBuilder.DefineMethod(methodInfo.Name
                , MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot |
                  MethodAttributes.Virtual | MethodAttributes.Final
                , methodInfo.ReturnType, Type.EmptyTypes);
            var ilGen = implMethod.GetILGenerator();
            ilGen.LoadArg(0);
            ilGen.FieldGet(sqlMapperField);
            ilGen.Return();
        }

        private void BuildMethod(Type interfaceType, TypeBuilder typeBuilder, MethodInfo methodInfo,
            FieldBuilder sqlMapperField, SmartSqlConfig smartSqlConfig, SqlMap sqlMap)
        {
            var returnType = methodInfo.ReturnType;
            var isTaskReturnType = CommonType.Task.IsAssignableFrom(returnType);
            Statement statement = PreStatement(interfaceType, sqlMap, methodInfo, returnType, isTaskReturnType,
                out var executeBehavior);

            var methodParams = methodInfo.GetParameters();
            var paramTypes = methodParams.Select(m => m.ParameterType).ToArray();
            if (paramTypes.Any(p => p.IsGenericParameter))
            {
                _logger.LogError(
                    "SmartSql.DyRepository method parameters do not support generic parameters for the time being!");
                throw new SmartSqlException(
                    "SmartSql.DyRepository method parameters do not support generic parameters for the time being!");
            }

            var implMethod = typeBuilder.DefineMethod(methodInfo.Name
                , MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot |
                  MethodAttributes.Virtual | MethodAttributes.Final
                , returnType, paramTypes);

            if (methodInfo.IsGenericMethod)
            {
                var genericArgs = methodInfo.GetGenericArguments();
                var gArgNames = genericArgs.Select(gArg => gArg.Name).ToArray();
                var defineGenericArgs = implMethod.DefineGenericParameters(gArgNames);
                for (int i = 0; i < gArgNames.Length; i++)
                {
                    var genericArg = genericArgs[i];
                    var defineGenericArg = defineGenericArgs[i];
                    defineGenericArg.SetGenericParameterAttributes(genericArg.GenericParameterAttributes);
                }
            }

            var ilGen = implMethod.GetILGenerator();
            bool onlyOneParam = paramTypes.Length == 1;
            Type firstParamType = paramTypes.FirstOrDefault();

            ilGen.DeclareLocal(RequestContextType.AbstractType);

            if (onlyOneParam && RequestContextType.AbstractType.IsAssignableFrom(firstParamType))
            {
                throw new SmartSqlException(
                    $"DyRepository.Method ParameterType :{firstParamType.FullName} can not be RequestContext.");
                //ilGen.LoadArg(1);
                //ilGen.StoreLocalVar(0);
            }

            bool isOnlyOneClassParam = onlyOneParam && !IsSimpleParam(firstParamType);
            EmitNewRequestContext(ilGen, isOnlyOneClassParam, firstParamType);
            EmitStatementAttr(ilGen, methodInfo);
            EmitSetTransaction(ilGen, methodInfo);
            EmitSetScope(ilGen, sqlMap.Scope);
            EmitSetSqlId(ilGen, statement);
            EmitSetCache(ilGen, methodInfo);

            if (isOnlyOneClassParam)
            {
                ilGen.LoadLocalVar(0);
                ilGen.LoadArg(1);
                ilGen.Callvirt(RequestContextType.Method.SetRequest);
            }
            else if (paramTypes.Length > 0)
            {
                ilGen.DeclareLocal(SqlParameterType.SqlParameterCollection);
                bool ignoreParameterCase = smartSqlConfig.Settings.IgnoreParameterCase;
                ilGen.Emit(ignoreParameterCase ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
                ilGen.New(SqlParameterType.Ctor.SqlParameterCollection);
                ilGen.StoreLocalVar(1);
                for (int i = 0; i < methodParams.Length; i++)
                {
                    int argIndex = i + 1;
                    var reqParam = methodParams[i];
                    string reqParamName = reqParam.Name;
                    var paramAttr = reqParam.GetCustomAttribute<ParamAttribute>();
                    if (paramAttr != null && !String.IsNullOrEmpty(paramAttr.Name))
                    {
                        reqParamName = paramAttr.Name;
                    }

                    ilGen.LoadLocalVar(1);
                    ilGen.LoadString(reqParamName);
                    ilGen.LoadArg(argIndex);

                    if (reqParam.ParameterType.IsValueType)
                    {
                        ilGen.Box(reqParam.ParameterType);
                    }

                    ilGen.LoadType(reqParam.ParameterType);
                    ilGen.New(SqlParameterType.Ctor.SqlParameter);

                    #region Ensure TypeHanlder

                    ilGen.Dup();
                    if (paramAttr != null && !String.IsNullOrEmpty(paramAttr.TypeHandler))
                    {
                        var typeHandlerField =
                            NamedTypeHandlerCache.GetTypeHandlerField(smartSqlConfig.Alias, paramAttr.TypeHandler);
                        if (typeHandlerField == null)
                        {
                            throw new SmartSqlException(
                                $"Can not find NamedTypeHandler SmartSql.Alias:[{smartSqlConfig.Alias}],Name :[{paramAttr.TypeHandler}].");
                        }

                        ilGen.FieldGet(typeHandlerField);
                    }
                    else
                    {
                        var getHandlerMethod = PropertyTypeHandlerCacheType.GetHandlerMethod(reqParam.ParameterType);
                        ilGen.Call(getHandlerMethod);
                    }

                    ilGen.Call(SqlParameterType.Method.SetTypeHandler);
                    ilGen.Call(SqlParameterType.Method.Add);

                    #endregion
                }

                ilGen.LoadLocalVar(0);
                ilGen.LoadLocalVar(1);
                ilGen.Callvirt(RequestContextType.Method.SetRequest);
            }

            MethodInfo executeMethod = PreExecuteMethod(executeBehavior, returnType, isTaskReturnType);
            ilGen.LoadArg(0);
            ilGen.FieldGet(sqlMapperField);
            ilGen.LoadLocalVar(0);
            ilGen.Callvirt(executeMethod);
            if (returnType == CommonType.Void)
            {
                ilGen.Pop();
            }

            ilGen.Return();
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(
                    $"RepositoryBuilder.BuildMethod:{methodInfo.Name}->Statement:[Scope:{sqlMap.Scope},Id:{statement.Id},Execute:{executeBehavior},IsAsync:{isTaskReturnType}]");
            }
        }

        private bool IsSimpleParam(Type paramType)
        {
            if (paramType == typeof(CommandType))
            {
                return false;
            }

            if (paramType == typeof(DataSourceChoice))
            {
                return false;
            }

            if (SqlParameterType.SqlParameterCollection == paramType)
            {
                return false;
            }

            if (paramType.IsValueType)
            {
                return true;
            }

            if (paramType == typeof(string))
            {
                return true;
            }

            if (paramType.IsGenericParameter)
            {
                return true;
            }

            return DataType.Enumerable.IsAssignableFrom(paramType);
        }

        #region Pre

        private string PreScope(Type interfaceType, string scope = "")
        {
            var sqlmapAttr = interfaceType.GetCustomAttribute<SqlMapAttribute>();
            if (sqlmapAttr != null && !string.IsNullOrEmpty(sqlmapAttr.Scope))
            {
                scope = sqlmapAttr.Scope;
            }
            else if (String.IsNullOrEmpty(scope))
            {
                scope = _templateParser.Parse(interfaceType.Name);
            }

            return scope;
        }

        private Statement PreStatement(Type interfaceType, SqlMap sqlMap, MethodInfo methodInfo,
            Type returnType, bool isTaskReturnType, out ExecuteBehavior executeBehavior)
        {
            var statementAttr = methodInfo.GetCustomAttribute<StatementAttribute>();
            var methodName = _sqlIdNamingConvert == null
                ? methodInfo.Name
                : _sqlIdNamingConvert.Invoke(interfaceType, methodInfo);

            if (isTaskReturnType && methodInfo.Name.EndsWith("Async") && _sqlIdNamingConvert == null)
            {
                methodName = methodName.Substring(0, methodName.Length - 5);
            }

            if (statementAttr != null)
            {
                statementAttr.Id = !String.IsNullOrEmpty(statementAttr.Id) ? statementAttr.Id : methodName;
            }
            else
            {
                statementAttr = new StatementAttribute
                {
                    Id = methodName
                };
            }

            var fullSqlId = $"{sqlMap.Scope}.{statementAttr.Id}";
            Statement statement;
            if (String.IsNullOrEmpty(statementAttr.Sql))
            {
                statement = sqlMap.GetStatement(fullSqlId);
            }
            else
            {
                if (sqlMap.Statements.ContainsKey(fullSqlId))
                {
                    throw new SmartSqlException($"Statement.FullSqlId:[{fullSqlId}] already exists!");
                }

                var resultCacheAttr = methodInfo.GetCustomAttribute<ResultCacheAttribute>();
                statement = new Statement
                {
                    SqlMap = sqlMap,
                    Id = statementAttr.Id,
                    StatementType = _statementAnalyzer.Analyse(statementAttr.Sql),
                    SqlTags = new List<ITag>
                    {
                        new SqlText(statementAttr.Sql, sqlMap.SmartSqlConfig.Database.DbProvider.ParameterPrefix)
                    },
                    CommandType = statementAttr.CommandType,
                    EnablePropertyChangedTrack = statementAttr.EnablePropertyChangedTrack,
                    ReadDb = statementAttr.ReadDb
                };
                if (statementAttr.CommandTimeout > 0)
                {
                    statement.CommandTimeout = statementAttr.CommandTimeout;
                }


                if (statementAttr.SourceChoice != DataSourceChoice.Unknow)
                {
                    statement.SourceChoice = statementAttr.SourceChoice;
                }

                if (resultCacheAttr != null)
                {
                    statement.CacheId = ParseCacheFullId(sqlMap.Scope, resultCacheAttr.CacheId);
                    statement.Cache = sqlMap.GetCache(statement.CacheId);
                }

                sqlMap.Statements.Add(statement.FullSqlId, statement);
            }

            returnType = isTaskReturnType ? returnType.GetGenericArguments().FirstOrDefault() : returnType;
            if (returnType == typeof(DataTable))
            {
                statementAttr.Execute = ExecuteBehavior.GetDataTable;
            }

            if (returnType == typeof(DataSet))
            {
                statementAttr.Execute = ExecuteBehavior.GetDataSet;
            }

            if (statementAttr.Execute == ExecuteBehavior.Auto)
            {
                if (CommonType.IsValueTuple(returnType))
                {
                    statementAttr.Execute = ExecuteBehavior.QuerySingle;
                }
                else if (returnType == CommonType.Int32 || returnType == CommonType.Void || returnType == null)
                {
                    statementAttr.Execute = ExecuteBehavior.Execute;
                    if (returnType == CommonType.Int32)
                    {
                        if (statement.StatementType.HasFlag(Configuration.StatementType.Select))
                        {
                            statementAttr.Execute = ExecuteBehavior.ExecuteScalar;
                        }
                    }
                }
                else if (returnType.IsValueType || returnType == CommonType.String)
                {
                    statementAttr.Execute = ExecuteBehavior.ExecuteScalar;
                    if (!statement.StatementType.HasFlag(Configuration.StatementType.Select))
                    {
                        statementAttr.Execute = ExecuteBehavior.Execute;
                    }
                }
                else
                {
                    var isQueryEnumerable = typeof(IEnumerable).IsAssignableFrom(returnType);
                    statementAttr.Execute = isQueryEnumerable ? ExecuteBehavior.Query : ExecuteBehavior.QuerySingle;
                }
            }

            executeBehavior = statementAttr.Execute;
            return statement;
        }

        private MethodInfo PreExecuteMethod(ExecuteBehavior executeBehavior, Type returnType, bool isTaskReturnType)
        {
            MethodInfo executeMethod;
            if (isTaskReturnType)
            {
                var realReturnType = returnType.GenericTypeArguments.FirstOrDefault();
                switch (executeBehavior)
                {
                    case ExecuteBehavior.Execute:
                    {
                        executeMethod = ISqlMapperType.Method.ExecuteAsync;
                        break;
                    }

                    case ExecuteBehavior.ExecuteScalar:
                    {
                        executeMethod = ISqlMapperType.Method.ExecuteScalarAsync.MakeGenericMethod(realReturnType);
                        break;
                    }

                    case ExecuteBehavior.QuerySingle:
                    {
                        executeMethod = ISqlMapperType.Method.QuerySingleAsync.MakeGenericMethod(realReturnType);
                        break;
                    }

                    case ExecuteBehavior.Query:
                    {
                        var method = ISqlMapperType.Method.QueryAsync;
                        var enumerableType = realReturnType.GenericTypeArguments[0];
                        executeMethod = method.MakeGenericMethod(enumerableType);
                        break;
                    }

                    case ExecuteBehavior.GetDataTable:
                    {
                        executeMethod = ISqlMapperType.Method.GetDataTableAsync;
                        break;
                    }

                    case ExecuteBehavior.GetDataSet:
                    {
                        executeMethod = ISqlMapperType.Method.GetDataSetAsync;
                        break;
                    }

                    default:
                    {
                        throw new ArgumentException();
                    }
                }
            }
            else
            {
                switch (executeBehavior)
                {
                    case ExecuteBehavior.Execute:
                    {
                        executeMethod = ISqlMapperType.Method.Execute;
                        break;
                    }

                    case ExecuteBehavior.ExecuteScalar:
                    {
                        executeMethod = ISqlMapperType.Method.ExecuteScalar.MakeGenericMethod(returnType);
                        break;
                    }

                    case ExecuteBehavior.QuerySingle:
                    {
                        executeMethod = ISqlMapperType.Method.QuerySingle.MakeGenericMethod(returnType);
                        break;
                    }

                    case ExecuteBehavior.Query:
                    {
                        var method = ISqlMapperType.Method.Query;
                        var enumerableType = returnType.GenericTypeArguments[0];
                        executeMethod = method.MakeGenericMethod(new Type[] {enumerableType});
                        break;
                    }

                    case ExecuteBehavior.GetDataTable:
                    {
                        executeMethod = ISqlMapperType.Method.GetDataTable;
                        break;
                    }

                    case ExecuteBehavior.GetDataSet:
                    {
                        executeMethod = ISqlMapperType.Method.GetDataSet;
                        break;
                    }

                    default:
                    {
                        throw new ArgumentException();
                    }
                }
            }

            return executeMethod;
        }

        #endregion

        #region Emit Set RequestContext

        private void EmitNewRequestContext(ILGenerator ilGen, bool isOnlyOneClassParam, Type onlyOneParamType)
        {
            var requestCtor = RequestContextType.Ctor;
            if (isOnlyOneClassParam)
            {
                requestCtor = RequestContextType.MakeGenericTypeCtor(onlyOneParamType);
            }

            ilGen.New(requestCtor);
            ilGen.StoreLocalVar(0);
        }

        private void EmitSetScope(ILGenerator ilGen, string scope)
        {
            ilGen.LoadLocalVar(0);
            ilGen.LoadString(scope);
            ilGen.Callvirt(RequestContextType.Method.SetScope);
        }

        private void EmitSetTransaction(ILGenerator ilGen, MethodInfo methodInfo)
        {
            IsolationLevel? isolationLevel = null;
            foreach (var attributeData in methodInfo.GetCustomAttributesData())
            {
                var attrType = attributeData.AttributeType;
                if (attrType == typeof(UseTransactionAttribute))
                {
                    var transactionAttribute = methodInfo.GetCustomAttribute<UseTransactionAttribute>();
                    isolationLevel = transactionAttribute.Level;
                    break;
                }

                #region  Warning:: SmartSql.AOP.TransactionAttribute , Suggested use UseTransactionAttribute

                if (attrType.FullName == "SmartSql.AOP.TransactionAttribute")
                {
                    _logger.LogWarning(
                        $"RepositoryBuilder.BuildMethod:{methodInfo.Name} Used:[{attrType.FullName}] ,Suggested use:[{typeof(UseTransactionAttribute).FullName}] For DyRepository interface.");
                    var transactionAttribute = methodInfo.GetCustomAttribute(attrType);
                    var transactionLevel = attrType.GetProperty("Level")?.GetValue(transactionAttribute);
                    if (transactionLevel != null)
                    {
                        isolationLevel = (IsolationLevel) transactionLevel;
                    }

                    break;
                }

                #endregion
            }

            if (isolationLevel.HasValue)
            {
                ilGen.LoadLocalVar(0);
                ilGen.LoadInt32(isolationLevel.GetHashCode());
                ilGen.New(NullableType<IsolationLevel>.Ctor);
                ilGen.Callvirt(RequestContextType.Method.SetTransaction);
            }
        }

        private static void EmitStatementAttr(ILGenerator ilGen, MethodInfo methodInfo)
        {
            var statementAttribute = methodInfo.GetCustomAttribute<StatementAttribute>();
            if (statementAttribute == null || !String.IsNullOrEmpty(statementAttribute.Sql)) return;

            if (statementAttribute.SourceChoice != DataSourceChoice.Unknow)
            {
                ilGen.LoadLocalVar(0);
                ilGen.LoadInt32(statementAttribute.SourceChoice.GetHashCode());
                ilGen.Callvirt(RequestContextType.Method.SetDataSourceChoice);
            }

            if (statementAttribute.CommandTimeout > 0)
            {
                ilGen.LoadLocalVar(0);
                ilGen.LoadInt32(statementAttribute.CommandTimeout);
                ilGen.Callvirt(RequestContextType.Method.SetCommandTimeout);
            }

            if (!String.IsNullOrEmpty(statementAttribute.ReadDb))
            {
                ilGen.LoadLocalVar(0);
                ilGen.LoadString(statementAttribute.ReadDb);
                ilGen.Callvirt(RequestContextType.Method.SetReadDb);
            }

            if (statementAttribute.EnablePropertyChangedTrack)
            {
                ilGen.LoadLocalVar(0);
                ilGen.LoadInt32(1);
                ilGen.Callvirt(RequestContextType.Method.SetEnablePropertyChangedTrack);
            }
        }

        private static void EmitSetCache(ILGenerator ilGen, MethodInfo methodInfo)
        {
            var resultCacheAttr = methodInfo.GetCustomAttribute<ResultCacheAttribute>();
            if (resultCacheAttr != null)
            {
                if (!String.IsNullOrEmpty(resultCacheAttr.Key))
                {
                    ilGen.LoadLocalVar(0);
                    ilGen.LoadString(resultCacheAttr.Key);
                    ilGen.Callvirt(RequestContextType.Method.SetCacheKeyTemplate);
                }
            }
        }

        private void EmitSetSqlId(ILGenerator ilGen, Statement statement)
        {
            ilGen.LoadLocalVar(0);
            ilGen.LoadString(statement.Id);
            ilGen.Callvirt(RequestContextType.Method.SetSqlId);
        }

        #endregion

        #region GetOrAddSqlMap

        private SqlMap GetOrAddSqlMap(SmartSqlConfig smartSqlConfig, Type interfaceType, string scope)
        {
            if (!smartSqlConfig.SqlMaps.TryGetValue(scope, out var sqlMap))
            {
                sqlMap = new SqlMap
                {
                    Path = interfaceType.AssemblyQualifiedName,
                    Scope = scope,
                    SmartSqlConfig = smartSqlConfig,
                    Statements = new Dictionary<string, Statement>(),
                    Caches = new Dictionary<string, Configuration.Cache>(),
                    MultipleResultMaps = new Dictionary<string, MultipleResultMap>(),
                    ParameterMaps = new Dictionary<string, ParameterMap>(),
                    ResultMaps = new Dictionary<string, ResultMap>()
                };
                smartSqlConfig.SqlMaps.Add(scope, sqlMap);
            }

            BuildCache(sqlMap, interfaceType);

            return sqlMap;
        }

        private void BuildCache(SqlMap sqlMap, Type interfaceType)
        {
            var cacheAttrs = interfaceType.GetCustomAttributes<CacheAttribute>();
            foreach (var cacheAttribute in cacheAttrs)
            {
                Configuration.Cache cache = new Configuration.Cache
                {
                    FlushInterval = new FlushInterval(), Id = ParseCacheFullId(sqlMap.Scope, cacheAttribute.Id)
                };
                if (sqlMap.Caches.ContainsKey(cache.Id))
                {
                    throw new SmartSqlException($"Cache.FullId:[{cache.Id}] already exists!");
                }

                if (cacheAttribute.FlushInterval > 0)
                {
                    cache.FlushInterval.Seconds = cacheAttribute.FlushInterval;
                }

                if (cacheAttribute.FlushOnExecutes != null && cacheAttribute.FlushOnExecutes.Length > 0)
                {
                    cache.FlushOnExecutes = cacheAttribute.FlushOnExecutes.Select(m => new FlushOnExecute
                    {
                        Statement = m.IndexOf('.') > 0 ? m : $"{sqlMap.Scope}.{m}"
                    }).ToList();
                }

                cache.Parameters = new Dictionary<string, object>
                {
                    {nameof(CacheAttribute.CacheSize), cacheAttribute.CacheSize}
                };
                cache.Type = cacheAttribute.Type;
                cache.Provider = CacheProviderUtil.Create(cache);
                sqlMap.Caches.Add(cache.Id, cache);
            }
        }

        private static String ParseCacheFullId(String scope, String cacheId)
        {
            if (cacheId.IndexOf('.') < 0)
            {
                return $"{scope}.{cacheId}";
            }
            else
            {
                return cacheId;
            }
        }

        #endregion

        public Type Build(Type interfaceType, SmartSqlConfig smartSqlConfig, string scope = "")
        {
            string implName = $"{interfaceType.Name.TrimStart('I')}_Impl_{Guid.NewGuid():N}";
            var typeBuilder = _moduleBuilder.DefineType(implName, TypeAttributes.Public);
            typeBuilder.AddInterfaceImplementation(interfaceType);
            typeBuilder.AddInterfaceImplementation(typeof(IRepositoryProxy));
            var sqlMapperField = typeBuilder.DefineField("sqlMapper", ISqlMapperType.Type, FieldAttributes.Family);
            var scopeField = typeBuilder.DefineField("scope", CommonType.String, FieldAttributes.Family);
            scope = PreScope(interfaceType, scope);
            var sqlMap = GetOrAddSqlMap(smartSqlConfig, interfaceType, scope);
            EmitBuildCtor(scope, typeBuilder, sqlMapperField, scopeField);
            var interfaceMethods = new List<MethodInfo>();

            var currentMethodInfos = interfaceType.GetMethods(BindingFlags.Instance | BindingFlags.Public);
            interfaceMethods.AddRange(currentMethodInfos);
            var currentIIs = interfaceType.GetInterfaces();
            foreach (var currentII in currentIIs)
            {
                var currentIIMethods = currentII.GetMethods(BindingFlags.Instance | BindingFlags.Public);
                interfaceMethods.AddRange(currentIIMethods);
            }

            foreach (var methodInfo in interfaceMethods)
            {
                if (methodInfo.ReturnType == ISqlMapperType.Type)
                {
                    BuildInternalGet(typeBuilder, methodInfo, sqlMapperField);
                }
                else
                {
                    BuildMethod(interfaceType, typeBuilder, methodInfo, sqlMapperField, smartSqlConfig, sqlMap);
                }
            }

            if (sqlMap.Path == interfaceType.AssemblyQualifiedName)
            {
                if (sqlMap.Caches?.Count > 0)
                {
                    smartSqlConfig.CacheManager.Reset();
                }
            }

            return typeBuilder.CreateTypeInfo();
        }
    }
}