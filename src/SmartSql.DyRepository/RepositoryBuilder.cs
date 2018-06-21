using Microsoft.Extensions.Logging;
using SmartSql.Abstractions;
using SmartSql.Exceptions;
using SmartSql.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartSql.DyRepository
{
    public class RepositoryBuilder : IRepositoryBuilder
    {
        private const string DEFAULT_SCOPE_TEMPLATE = "I{Scope}Repository";

        private Regex _repositoryScope;

        private AssemblyBuilder _assemblyBuilder;
        private ModuleBuilder _moduleBuilder;

        public RepositoryBuilder(
             string scope_template
            , ILogger<RepositoryBuilder> logger
            )
        {
            InitScopeTemlate(scope_template);
            InitAssembly();
            _logger = logger;
        }

        private void InitScopeTemlate(string template = "")
        {
            if (String.IsNullOrEmpty(template))
            {
                template = DEFAULT_SCOPE_TEMPLATE;
            }
            template = template.Replace("{Scope}", @"([\p{L}\p{N}_]+)");
            template = template.Insert(0, "^");
            template = template + "$";
            _repositoryScope = new Regex(template, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        }

        private String GetScope(string repositoryName)
        {
            var matchScope = _repositoryScope.Match(repositoryName);
            return matchScope.Groups[1].Value;
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

        /// <summary>
        /// 构建仓储接口实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Type BuildRepositoryImpl(Type interfaceType)
        {
            string implName = interfaceType.Name.TrimStart('I') + "_Impl";
            var typeBuilder = _moduleBuilder.DefineType(implName, TypeAttributes.Public);
            typeBuilder.AddInterfaceImplementation(interfaceType);
            var sqlMapperField = typeBuilder.DefineField("sqlMapper", typeof(ISmartSqlMapper), FieldAttributes.Family);
            var scopeField = typeBuilder.DefineField("scope", typeof(string), FieldAttributes.Family);
            string scope = PreScoe(interfaceType);
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
                BuildMethod(typeBuilder, methodInfo, sqlMapperField, scope);
            }
            return typeBuilder.CreateTypeInfo();
        }
        private void EmitBuildCtor(string scope, TypeBuilder typeBuilder, FieldBuilder sqlMapperField, FieldBuilder scopeField)
        {
            var paramTypes = new Type[] { typeof(ISmartSqlMapper) };
            var ctorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public, CallingConventions.Standard, paramTypes);
            var ctorIL = ctorBuilder.GetILGenerator();

            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Call, (typeof(object).GetConstructor(Type.EmptyTypes)));
            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Ldarg_1);
            ctorIL.Emit(OpCodes.Stfld, sqlMapperField);
            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Ldstr, scope);
            ctorIL.Emit(OpCodes.Stfld, scopeField);
            ctorIL.Emit(OpCodes.Ret);
        }

        private string PreScoe(Type interfaceType)
        {
            var sqlmapAttr = interfaceType.GetCustomAttribute<SqlMapAttribute>();
            string scope = String.Empty;
            if (sqlmapAttr != null)
            {
                scope = !String.IsNullOrEmpty(sqlmapAttr.Scope) ? sqlmapAttr.Scope : GetScope(interfaceType.Name);
            }
            else
            {
                scope = GetScope(interfaceType.Name);
            }
            return scope;
        }
        private readonly static Type _reqContextType = typeof(RequestContext);
        private readonly static Type _taskType = typeof(Task);
        private readonly static Type _voidType = typeof(void);
        private readonly static Type _reqParamsDicType = typeof(Dictionary<string, object>);

        private readonly static ConstructorInfo _reqContextCtor = _reqContextType.GetConstructor(Type.EmptyTypes);
        private readonly static MethodInfo _set_DataSourceChoiceMethod = _reqContextType.GetMethod("set_DataSourceChoice");
        private readonly static MethodInfo _set_CommandTypeMethod = _reqContextType.GetMethod("set_CommandType");
        private readonly static MethodInfo _set_ScopeMethod = _reqContextType.GetMethod("set_Scope");
        private readonly static MethodInfo _set_SqlIdMethod = _reqContextType.GetMethod("set_SqlId");
        private readonly static MethodInfo _set_RequestMethod = _reqContextType.GetMethod("set_Request");
        private readonly static MethodInfo _set_RealSqlMethod = _reqContextType.GetMethod("set_RealSql");

        private readonly static ConstructorInfo _reqParamsDicCtor = _reqParamsDicType.GetConstructor(Type.EmptyTypes);
        private readonly static MethodInfo _addReqParamMehtod = _reqParamsDicType.GetMethod("Add");
        private readonly ILogger<RepositoryBuilder> _logger;

        private void BuildMethod(TypeBuilder typeBuilder, MethodInfo methodInfo, FieldBuilder sqlMapperField, string scope)
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

            var isTaskReturnType = _taskType.IsAssignableFrom(returnType);

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

            StatementAttribute statementAttr = PreStatement(methodInfo, returnType, isTaskReturnType);
            var ilGenerator = implMehtod.GetILGenerator();
            ilGenerator.DeclareLocal(_reqContextType);
            ilGenerator.DeclareLocal(_reqParamsDicType);

            if (paramTypes.Length == 1 && paramTypes.First() == _reqContextType)
            {
                ilGenerator.Emit(OpCodes.Ldarg_1);
                ilGenerator.Emit(OpCodes.Stloc_0);
            }
            else
            {
                EmitNewRequestContext(ilGenerator);
                if (String.IsNullOrEmpty(statementAttr.Sql))
                {
                    EmitSetScope(ilGenerator, scope);
                    EmitSetSqlId(ilGenerator, statementAttr);
                }
                else
                {
                    EmitSetRealSql(ilGenerator, statementAttr);
                }
                if (paramTypes.Length == 1 && !IsSimpleParam(paramTypes.First()))
                {
                    ilGenerator.Emit(OpCodes.Ldloc_0);
                    ilGenerator.Emit(OpCodes.Ldarg_1);
                    ilGenerator.Emit(OpCodes.Call, _set_RequestMethod);
                }
                else if (paramTypes.Length > 0)
                {
                    ilGenerator.Emit(OpCodes.Newobj, _reqParamsDicCtor);
                    ilGenerator.Emit(OpCodes.Stloc_1);
                    for (int i = 0; i < methodParams.Length; i++)
                    {
                        int argIndex = i + 1;
                        var reqParam = methodParams[i];
                        if (reqParam.ParameterType == typeof(DataSourceChoice))
                        {
                            ilGenerator.Emit(OpCodes.Ldloc_0);
                            EmitUtils.LoadArg(ilGenerator, argIndex);
                            ilGenerator.Emit(OpCodes.Call, _set_DataSourceChoiceMethod);
                            continue;
                        }
                        if (reqParam.ParameterType == typeof(CommandType))
                        {
                            ilGenerator.Emit(OpCodes.Ldloc_0);
                            EmitUtils.LoadArg(ilGenerator, argIndex);
                            ilGenerator.Emit(OpCodes.Call, _set_CommandTypeMethod);
                            continue;
                        }
                        ilGenerator.Emit(OpCodes.Ldloc_1); //[dic]
                        ilGenerator.Emit(OpCodes.Ldstr, reqParam.Name);//[dic][param-name]
                        EmitUtils.LoadArg(ilGenerator, argIndex);
                        if (reqParam.ParameterType.IsValueType)
                        {
                            ilGenerator.Emit(OpCodes.Box, reqParam.ParameterType);
                        }
                        ilGenerator.Emit(OpCodes.Call, _addReqParamMehtod);//[empty]
                    }
                    ilGenerator.Emit(OpCodes.Ldloc_0);
                    ilGenerator.Emit(OpCodes.Ldloc_1);
                    ilGenerator.Emit(OpCodes.Call, _set_RequestMethod);
                }
            }

            MethodInfo executeMethod = null;
            executeMethod = PreExecuteMethod(statementAttr, returnType, isTaskReturnType);
            ilGenerator.Emit(OpCodes.Ldarg_0);// [this]
            ilGenerator.Emit(OpCodes.Ldfld, sqlMapperField);//[this][sqlMapper]
            ilGenerator.Emit(OpCodes.Ldloc_0);//[sqlMapper][requestContext]
            ilGenerator.Emit(OpCodes.Call, executeMethod);
            if (returnType == _voidType)
            {
                ilGenerator.Emit(OpCodes.Pop);
            }
            ilGenerator.Emit(OpCodes.Ret);

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"RepositoryBuilder.BuildMethod:{methodInfo.Name}->Statement:[Scope:{scope},Id:{statementAttr.Id},Execute:{statementAttr.Execute},Sql:{statementAttr.Sql},IsAsync:{isTaskReturnType}]");
            }
        }

        private void EmitNewRequestContext(ILGenerator ilGenerator)
        {
            ilGenerator.Emit(OpCodes.Newobj, _reqContextCtor);
            ilGenerator.Emit(OpCodes.Stloc_0);
        }

        private void EmitSetRealSql(ILGenerator ilGenerator, StatementAttribute statementAttr)
        {
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.Emit(OpCodes.Ldstr, statementAttr.Sql);
            ilGenerator.Emit(OpCodes.Call, _set_RealSqlMethod);
        }
        private void EmitSetSqlId(ILGenerator ilGenerator, StatementAttribute statementAttr)
        {
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.Emit(OpCodes.Ldstr, statementAttr.Id);
            ilGenerator.Emit(OpCodes.Call, _set_SqlIdMethod);
        }

        private void EmitSetScope(ILGenerator ilGenerator, string scope)
        {
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.Emit(OpCodes.Ldstr, scope);
            ilGenerator.Emit(OpCodes.Call, _set_ScopeMethod);
        }

        private StatementAttribute PreStatement(MethodInfo methodInfo, Type returnType, bool isTaskReturnType)
        {
            returnType = isTaskReturnType ? returnType.GetGenericArguments().FirstOrDefault() : returnType;
            var statementAttr = methodInfo.GetCustomAttribute<StatementAttribute>();
            var methodName = methodInfo.Name;
            if (isTaskReturnType && methodInfo.Name.EndsWith("Async"))
            {
                methodName = methodName.Substring(0, methodName.Length - 5);
            }

            if (statementAttr != null)
            {
                statementAttr.Id = !String.IsNullOrEmpty(statementAttr.Id) ? statementAttr.Id
                    : methodName;
            }
            else
            {
                statementAttr = new StatementAttribute
                {
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
                if (returnType == typeof(int) || returnType == _voidType || returnType == null)
                {
                    statementAttr.Execute = ExecuteBehavior.Execute;
                }
                else if (returnType.IsValueType || returnType == typeof(string))
                {
                    statementAttr.Execute = ExecuteBehavior.ExecuteScalar;
                }
                else
                {
                    var isQueryEnumerable = typeof(IEnumerable).IsAssignableFrom(returnType);
                    if (isQueryEnumerable)
                    {
                        statementAttr.Execute = ExecuteBehavior.Query;
                    }
                    else
                    {
                        statementAttr.Execute = ExecuteBehavior.QuerySingle;
                    }
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
                            executeMethod = typeof(ISmartSqlMapperAsync).GetMethod("ExecuteAsync", new Type[] { typeof(RequestContext) });
                            break;
                        }
                    case ExecuteBehavior.ExecuteScalar:
                        {
                            var method = typeof(ISmartSqlMapperAsync).GetMethod("ExecuteScalarAsync", new Type[] { typeof(RequestContext) });

                            executeMethod = method.MakeGenericMethod(new Type[] { realReturnType });
                            break;
                        }
                    case ExecuteBehavior.QuerySingle:
                        {
                            var method = typeof(ISmartSqlMapperAsync).GetMethod("QuerySingleAsync", new Type[] { typeof(RequestContext) });
                            executeMethod = method.MakeGenericMethod(new Type[] { realReturnType });
                            break;
                        }
                    case ExecuteBehavior.Query:
                        {
                            var method = typeof(ISmartSqlMapperAsync).GetMethod("QueryAsync", new Type[] { typeof(RequestContext) });
                            var enumerableType = realReturnType.GenericTypeArguments[0];
                            executeMethod = method.MakeGenericMethod(new Type[] { enumerableType });
                            break;
                        }
                    case ExecuteBehavior.GetDataTable:
                        {
                            executeMethod = typeof(ISmartSqlMapperAsync).GetMethod("GetDataTableAsync", new Type[] { typeof(RequestContext) });
                            break;
                        }
                    case ExecuteBehavior.GetDataSet:
                        {
                            executeMethod = typeof(ISmartSqlMapperAsync).GetMethod("GetDataSetAsync", new Type[] { typeof(RequestContext) });
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
                            executeMethod = typeof(ISmartSqlMapper).GetMethod("Execute", new Type[] { typeof(RequestContext) });
                            break;
                        }
                    case ExecuteBehavior.ExecuteScalar:
                        {
                            var method = typeof(ISmartSqlMapper).GetMethod("ExecuteScalar", new Type[] { typeof(RequestContext) });
                            executeMethod = method.MakeGenericMethod(new Type[] { returnType });
                            break;
                        }
                    case ExecuteBehavior.QuerySingle:
                        {
                            var method = typeof(ISmartSqlMapper).GetMethod("QuerySingle", new Type[] { typeof(RequestContext) });
                            executeMethod = method.MakeGenericMethod(new Type[] { returnType });
                            break;
                        }
                    case ExecuteBehavior.Query:
                        {
                            var method = typeof(ISmartSqlMapper).GetMethod("Query", new Type[] { typeof(RequestContext) });
                            var enumerableType = returnType.GenericTypeArguments[0];
                            executeMethod = method.MakeGenericMethod(new Type[] { enumerableType });
                            break;
                        }
                    case ExecuteBehavior.GetDataTable:
                        {
                            executeMethod = typeof(ISmartSqlMapper).GetMethod("GetDataTable", new Type[] { typeof(RequestContext) });
                            break;
                        }
                    case ExecuteBehavior.GetDataSet:
                        {
                            executeMethod = typeof(ISmartSqlMapper).GetMethod("GetDataSet", new Type[] { typeof(RequestContext) });
                            break;
                        }
                    default: { throw new ArgumentException(); }
                }
            }

            return executeMethod;
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
            return typeof(IEnumerable).IsAssignableFrom(paramType);
        }
    }
}
