using SmartSql.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;

namespace SmartSql.DyRespository
{
    public class RespositoryBuilder : IRespositoryBuilder
    {
        private const string DEFAULT_SCOPE_TEMPLATE = "I{Scope}Respository";

        private Regex respositoryScope;

        private AssemblyBuilder assemblyBuilder;
        private ModuleBuilder moduleBuilder;
        public RespositoryBuilder(string scope_template = "")
        {
            InitScopeTemlate(scope_template);
            InitAssembly();
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
            respositoryScope = new Regex(template, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        }

        private String GetScope(string respositoryName)
        {
            var matchScope = respositoryScope.Match(respositoryName);
            return matchScope.Groups[1].Value;
        }

        private void InitAssembly()
        {
            string assemblyName = "SmartSql.RespositoryImpl" + this.GetHashCode();
            assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName
            {
                Name = assemblyName
            }, AssemblyBuilderAccess.Run);
            moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName + ".dll");
        }

        /// <summary>
        /// 构建仓储接口实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Type BuildRespositoryImpl(Type interfaceType)
        {
            string implName = interfaceType.Name.TrimStart('I') + "_Impl";
            var typeBuilder = moduleBuilder.DefineType(implName, TypeAttributes.Public);
            typeBuilder.AddInterfaceImplementation(interfaceType);
            var sqlMapperField = typeBuilder.DefineField("sqlMapper", typeof(ISmartSqlMapper), FieldAttributes.Family);
            var scopeField = typeBuilder.DefineField("scope", typeof(string), FieldAttributes.Family);
            BuildCtor(interfaceType, typeBuilder, sqlMapperField, scopeField);
            var interfaceMethods = interfaceType.GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (var methodInfo in interfaceMethods)
            {
                BuildMethod(typeBuilder, methodInfo, sqlMapperField, scopeField);
            }
            return typeBuilder.CreateTypeInfo();
        }
        private void BuildCtor(Type interfaceType, TypeBuilder typeBuilder, FieldBuilder sqlMapperField, FieldBuilder scopeField)
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
            string scope = PreScoe(interfaceType);
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

        private void BuildMethod(TypeBuilder typeBuilder, MethodInfo methodInfo, FieldBuilder sqlMapperField, FieldBuilder scopeField)
        {
            var paramType = methodInfo.GetParameters()[0].ParameterType;
            var returnType = methodInfo.ReturnType;
            var implMehtod = typeBuilder.DefineMethod(methodInfo.Name
                , MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final
                , returnType, new Type[] { paramType });

            StatementAttribute statementAttr = PreStatement(methodInfo, returnType);
            var ilGenerator = implMehtod.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, sqlMapperField);
            var reqType = typeof(RequestContext);
            var ctorReq = typeof(RequestContext).GetConstructor(Type.EmptyTypes);
            ilGenerator.Emit(OpCodes.Newobj, ctorReq);
            ilGenerator.Emit(OpCodes.Dup);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, scopeField);
            var set_ScopeMethod = reqType.GetMethod("set_Scope");
            ilGenerator.Emit(OpCodes.Callvirt, set_ScopeMethod);
            ilGenerator.Emit(OpCodes.Dup);
            ilGenerator.Emit(OpCodes.Ldstr, statementAttr.Id);
            var set_SqlIdMethod = reqType.GetMethod("set_SqlId");
            ilGenerator.Emit(OpCodes.Callvirt, set_SqlIdMethod);
            ilGenerator.Emit(OpCodes.Dup);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            var set_RequestMethod = reqType.GetMethod("set_Request");
            ilGenerator.Emit(OpCodes.Callvirt, set_RequestMethod);
            MethodInfo executeMethod = null;
            executeMethod = PreExecuteMethod(statementAttr, returnType);
            ilGenerator.Emit(OpCodes.Callvirt, executeMethod);
            ilGenerator.Emit(OpCodes.Ret);
        }

        private StatementAttribute PreStatement(MethodInfo methodInfo, Type returnType)
        {
            var statementAttr = methodInfo.GetCustomAttribute<StatementAttribute>();
            if (statementAttr != null)
            {
                statementAttr.Id = !String.IsNullOrEmpty(statementAttr.Id) ? statementAttr.Id : methodInfo.Name;
            }
            else
            {
                statementAttr = new StatementAttribute
                {
                    Id = methodInfo.Name
                };
            }
            if (statementAttr.Execute == ExecuteBehavior.Auto)
            {
                if (returnType == typeof(int))
                {
                    statementAttr.Execute = ExecuteBehavior.Execute;
                }
                else if (returnType.IsValueType)
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

        private static MethodInfo PreExecuteMethod(StatementAttribute statementAttr, Type returnType)
        {
            MethodInfo executeMethod;

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
                default: { throw new ArgumentException(); }
            }
            return executeMethod;
        }
    }
}
