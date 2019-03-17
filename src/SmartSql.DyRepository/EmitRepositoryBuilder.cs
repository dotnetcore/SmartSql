using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.Extensions.Logging;
using SmartSql.Configuration;
using SmartSql.DataSource;
using SmartSql.DyRepository.Annotations;
using SmartSql.Exceptions;
using SmartSql.Reflection.TypeConstants;
using SmartSql.Utils;

namespace SmartSql.DyRepository
{
    public class EmitRepositoryBuilder : IRepositoryBuilder
    {
        private ScopeTemplateParser _templateParser;
        private AssemblyBuilder _assemblyBuilder;
        private ModuleBuilder _moduleBuilder;
        private Func<Type, MethodInfo, String> _sqlIdNamingConvert;
        private readonly ILogger _logger;
        private readonly StatementAnalyzer _statementAnalyzer;
        public EmitRepositoryBuilder(
             string scope_template
            , Func<Type, MethodInfo, String> sqlIdNamingConvert
            , ILogger logger
            )
        {
            _sqlIdNamingConvert = sqlIdNamingConvert;
            _logger = logger;
            _templateParser = new ScopeTemplateParser(scope_template);
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
        private void EmitBuildCtor(string scope, TypeBuilder typeBuilder, FieldBuilder sqlMapperField, FieldBuilder scopeField)
        {
            var paramTypes = new Type[] { ISqlMapperType.Type };
            var ctorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public, CallingConventions.Standard, paramTypes);
            var ilGen = ctorBuilder.GetILGenerator();
            ilGen.LoadArg(0);
            ilGen.Emit(OpCodes.Call, CommonType.Object.GetConstructor(Type.EmptyTypes));
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
            var implMehtod = typeBuilder.DefineMethod(methodInfo.Name
            , MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final
            , methodInfo.ReturnType, Type.EmptyTypes);
            var ilGen = implMehtod.GetILGenerator();
            ilGen.LoadArg(0);
            ilGen.FieldSet(sqlMapperField);
            ilGen.Return();
        }
        private void BuildMethod(Type interfaceType, TypeBuilder typeBuilder, MethodInfo methodInfo, FieldBuilder sqlMapperField, SmartSqlConfig smartSqlConfig, string scope)
        {
            var methodParams = methodInfo.GetParameters();
            var paramTypes = methodParams.Select(m => m.ParameterType).ToArray();
            if (paramTypes.Any(p => p.IsGenericParameter))
            {
                _logger.LogError("SmartSql.DyRepository method parameters do not support generic parameters for the time being!");
                throw new SmartSqlException("SmartSql.DyRepository method parameters do not support generic parameters for the time being!");
            }
            var returnType = methodInfo.ReturnType;

            var implMehtod = typeBuilder.DefineMethod(methodInfo.Name
                , MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final
                , returnType, paramTypes);

            var isTaskReturnType = CommonType.Task.IsAssignableFrom(returnType);

            if (methodInfo.IsGenericMethod)
            {
                var genericArgs = methodInfo.GetGenericArguments();
                var gArgNames = genericArgs.Select(gArg => gArg.Name).ToArray();
                var defineGenericArgs = implMehtod.DefineGenericParameters(gArgNames);
                for (int i = 0; i < gArgNames.Length; i++)
                {
                    var genericArg = genericArgs[i];
                    var defineGenericArg = defineGenericArgs[i];
                    defineGenericArg.SetGenericParameterAttributes(genericArg.GenericParameterAttributes);
                }
            }

            StatementAttribute statementAttr = PreStatement(interfaceType, scope, methodInfo, returnType, isTaskReturnType, smartSqlConfig);
            var ilGen = implMehtod.GetILGenerator();
            ilGen.DeclareLocal(RequestContextType.Type);
            ilGen.DeclareLocal(SqlParameterType.SqlParameterCollection);
            if (paramTypes.Length == 1 && paramTypes.First() == RequestContextType.Type)
            {
                ilGen.LoadArg(1);
                ilGen.StoreLocalVar(0);
            }
            else
            {
                EmitNewRequestContext(ilGen);
                EmitSetCommandType(ilGen, statementAttr);
                EmitSetDataSourceChoice(ilGen, statementAttr);

                if (String.IsNullOrEmpty(statementAttr.Sql))
                {
                    EmitSetScope(ilGen, statementAttr.Scope);
                    EmitSetSqlId(ilGen, statementAttr);
                }
                else
                {
                    EmitSetRealSql(ilGen, statementAttr);
                }
                if (paramTypes.Length == 1 && !IsSimpleParam(paramTypes.First()))
                {
                    ilGen.LoadLocalVar(0);
                    ilGen.LoadArg(1);
                    ilGen.Call(RequestContextType.Method.SetRequest);
                }
                else if (paramTypes.Length > 0)
                {
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
                        ilGen.Call(SqlParameterType.Method.Add);
                    }
                    ilGen.LoadLocalVar(0);
                    ilGen.LoadLocalVar(1);
                    ilGen.Call(RequestContextType.Method.SetRequest);
                }
            }
            MethodInfo executeMethod = PreExecuteMethod(statementAttr, returnType, isTaskReturnType);
            ilGen.LoadArg(0);
            ilGen.FieldGet(sqlMapperField);
            ilGen.LoadLocalVar(0);
            ilGen.Call(executeMethod);
            if (returnType == CommonType.Void)
            {
                ilGen.Pop();
            }
            ilGen.Return();
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"RepositoryBuilder.BuildMethod:{methodInfo.Name}->Statement:[Scope:{statementAttr.Scope},Id:{statementAttr.Id},Execute:{statementAttr.Execute},Sql:{statementAttr.Sql},IsAsync:{isTaskReturnType}]");
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
            if (paramType.IsValueType) { return true; }
            if (paramType == typeof(string)) { return true; }
            if (paramType.IsGenericParameter) { return true; }
            return DataType.Enumerable.IsAssignableFrom(paramType);
        }
        #region Pre
        private string PreScoe(Type interfaceType, string scope = "")
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
        private StatementAttribute PreStatement(Type interfaceType, string scope, MethodInfo methodInfo, Type returnType, bool isTaskReturnType, SmartSqlConfig smartSqlConfig)
        {
            returnType = isTaskReturnType ? returnType.GetGenericArguments().FirstOrDefault() : returnType;
            var statementAttr = methodInfo.GetCustomAttribute<StatementAttribute>();

            var methodName = _sqlIdNamingConvert == null ? methodInfo.Name : _sqlIdNamingConvert.Invoke(interfaceType, methodInfo);

            if (isTaskReturnType && methodInfo.Name.EndsWith("Async") && _sqlIdNamingConvert == null)
            {
                methodName = methodName.Substring(0, methodName.Length - 5);
            }
            if (statementAttr != null)
            {
                statementAttr.Id = !String.IsNullOrEmpty(statementAttr.Id) ? statementAttr.Id : methodName;
                statementAttr.Scope = !String.IsNullOrEmpty(statementAttr.Scope) ? statementAttr.Scope : scope;
            }
            else
            {
                statementAttr = new StatementAttribute
                {
                    Scope = scope,
                    Id = methodName
                };
            }

            if (returnType == typeof(DataTable))
            {
                statementAttr.Execute = ExecuteBehavior.GetDataTable;
                return statementAttr;
            }
            if (returnType == typeof(DataSet))
            {
                statementAttr.Execute = ExecuteBehavior.GetDataSet;
                return statementAttr;
            }

            if (statementAttr.Execute == ExecuteBehavior.Auto)
            {
                Configuration.StatementType statementType = Configuration.StatementType.Unknown;
                if (String.IsNullOrEmpty(statementAttr.Sql))
                {
                    var sqlStatement = smartSqlConfig.GetSqlMap(statementAttr.Scope).GetStatement($"{statementAttr.Scope}.{statementAttr.Id}");
                    statementType = sqlStatement.StatementType;
                }
                else
                {
                    statementType = _statementAnalyzer.Analyse(statementAttr.Sql);
                }

                if (returnType == CommonType.Int32 || returnType == CommonType.Void || returnType == null)
                {
                    statementAttr.Execute = ExecuteBehavior.Execute;
                    if (returnType == CommonType.Int32)
                    {
                        if (statementType.HasFlag(Configuration.StatementType.Select))
                        {
                            statementAttr.Execute = ExecuteBehavior.ExecuteScalar;
                        }
                    }
                }
                else if (returnType.IsValueType || returnType == CommonType.String)
                {
                    statementAttr.Execute = ExecuteBehavior.ExecuteScalar;
                    if (!statementType.HasFlag(Configuration.StatementType.Select))
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
            return statementAttr;
        }

        private MethodInfo PreExecuteMethod(StatementAttribute statementAttr, Type returnType, bool isTaskReturnType)
        {
            MethodInfo executeMethod;
            if (isTaskReturnType)
            {
                var realReturnType = returnType.GenericTypeArguments.FirstOrDefault();
                switch (statementAttr.Execute)
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
                    default: { throw new ArgumentException(); }
                }
            }
            else
            {
                switch (statementAttr.Execute)
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
                            executeMethod = method.MakeGenericMethod(new Type[] { enumerableType });
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
                    default: { throw new ArgumentException(); }
                }
            }

            return executeMethod;
        }
        #endregion
        #region Emit Set RequestContext
        private void EmitNewRequestContext(ILGenerator ilGen)
        {
            ilGen.New(RequestContextType.Ctor);
            ilGen.StoreLocalVar(0);
        }
        private void EmitSetScope(ILGenerator ilGen, string scope)
        {
            ilGen.LoadLocalVar(0);
            ilGen.LoadString(scope);
            ilGen.Call(RequestContextType.Method.SetScope);
        }
        private static void EmitSetCommandType(ILGenerator ilGen, StatementAttribute statementAttr)
        {
            if (statementAttr.CommandType != CommandType.Text)
            {
                ilGen.LoadLocalVar(0);
                ilGen.LoadInt32(statementAttr.CommandType.GetHashCode());
                ilGen.Call(RequestContextType.Method.SetCommandType);
            }
        }
        private static void EmitSetDataSourceChoice(ILGenerator ilGen, StatementAttribute statementAttr)
        {
            if (statementAttr.SourceChoice != DataSourceChoice.Unknow)
            {
                ilGen.LoadLocalVar(0);
                ilGen.LoadInt32(statementAttr.SourceChoice.GetHashCode());
                ilGen.Call(RequestContextType.Method.SetDataSourceChoice);
            }
        }
        private void EmitSetRealSql(ILGenerator ilGen, StatementAttribute statementAttr)
        {
            ilGen.LoadLocalVar(0);
            ilGen.LoadString(statementAttr.Sql);
            ilGen.Call(RequestContextType.Method.SetRealSql);
        }
        private void EmitSetSqlId(ILGenerator ilGen, StatementAttribute statementAttr)
        {
            ilGen.LoadLocalVar(0);
            ilGen.LoadString(statementAttr.Id);
            ilGen.Call(RequestContextType.Method.SetSqlId);
        }

        #endregion
        public Type Build(Type interfaceType, SmartSqlConfig smartSqlConfig, string scope = "")
        {
            string implName = $"{interfaceType.Name.TrimStart('I')}_Impl_{Guid.NewGuid().ToString("N")}";
            var typeBuilder = _moduleBuilder.DefineType(implName, TypeAttributes.Public);
            typeBuilder.AddInterfaceImplementation(interfaceType);
            var sqlMapperField = typeBuilder.DefineField("sqlMapper", ISqlMapperType.Type, FieldAttributes.Family);
            var scopeField = typeBuilder.DefineField("scope", CommonType.String, FieldAttributes.Family);
            scope = PreScoe(interfaceType, scope);
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
                    BuildMethod(interfaceType, typeBuilder, methodInfo, sqlMapperField, smartSqlConfig, scope);
                }
            }
            return typeBuilder.CreateTypeInfo();
        }
    }
}
