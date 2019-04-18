using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using SmartSql.Exceptions;
namespace SmartSql.TypeHandlers
{
    public static class NamedTypeHandlerCache
    {
        private static readonly object s_lock = new object();
        private static readonly ModuleBuilder _moduleBuilder;
        private static readonly Dictionary<string, Dictionary<String, FieldInfo>> _mappedField = new Dictionary<string, Dictionary<string, FieldInfo>>();
        static NamedTypeHandlerCache()
        {
            string assemblyName = "SmartSql.NamedTypeHandlerCache";
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName
            {
                Name = assemblyName
            }, AssemblyBuilderAccess.Run);
            _moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName + ".dll");
        }

        private static string GetCacheTypeName(string alias)
        {
            return $"SmartSql.NamedTypeHandlerCache.{alias}";
        }

        private static TypeBuilder CreateCacheTypeBuilder(string alias)
        {
            var typeName = GetCacheTypeName(alias);
            return _moduleBuilder.DefineType(typeName, TypeAttributes.Class | TypeAttributes.Public);
        }

        public static void Build(string alias, IDictionary<string, ITypeHandler> namedTypeHandlers)
        {
            lock (s_lock)
            {
                if (_mappedField.ContainsKey(alias))
                {
                    throw new SmartSqlException($"NamedTypeHandlerCache.Alias:[{alias}]Already exist.");
                }
                var mappedFieldInfos = new Dictionary<string, FieldInfo>();
                _mappedField.Add(alias, mappedFieldInfos);
                var cacheTypeBuilder = CreateCacheTypeBuilder(alias);

                foreach (var namedTypeHandler in namedTypeHandlers)
                {
                    cacheTypeBuilder.DefineField(namedTypeHandler.Key
                        , typeof(ITypeHandler), FieldAttributes.Public | FieldAttributes.Static);
                }
                var typeInfo = cacheTypeBuilder.CreateTypeInfo();

                foreach (var namedTypeHandler in namedTypeHandlers)
                {
                    var field = typeInfo.GetField(namedTypeHandler.Key);
                    field.SetValue(null, namedTypeHandler.Value);
                    mappedFieldInfos.Add(namedTypeHandler.Key, field);
                }
            }
        }

        public static FieldInfo GetTypeHandlerField(string alias, string typeHandlerName)
        {
            if (!_mappedField.TryGetValue(alias, out var mappedFieldInfos))
            {
                throw new SmartSqlException($"Can not find NamedTypeHandlerCache.Alias:{alias}.");
            }
            if (!mappedFieldInfos.TryGetValue(typeHandlerName, out var typeHandlerFieldInfo))
            {
                throw new SmartSqlException($"Can not find NamedTypeHandlerCache.Alias:{alias} TypeHandler.Name:{typeHandlerName}.");
            }
            return typeHandlerFieldInfo;
        }
    }
}
