using SmartSql.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public class TypeHandlerCacheBuilder
    {
        private AssemblyBuilder _assemblyBuilder;
        private ModuleBuilder _moduleBuilder;
        private void Init()
        {
            string assemblyName = "SmartSql.TypeHandlerCacheBuilder" + this.GetHashCode();
            _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName
            {
                Name = assemblyName
            }, AssemblyBuilderAccess.Run);
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule(assemblyName + ".dll");
        }

        public string CreateNameTypeHandlerCacheClassName(string typeHandlerName)
        {
            return $"{typeHandlerName}_TypeHandler_{this.GetHashCode()}";
        }

        public Type CreateNameTypeHandler(TypeHandler typeHandler)
        {
            string className = CreateNameTypeHandlerCacheClassName(typeHandler.Name);
            var typeBuilder = _moduleBuilder.DefineType(className, TypeAttributes.Public);
            
            throw new NotImplementedException();
        }
    }
}
