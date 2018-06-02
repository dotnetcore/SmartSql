using SmartSql.Abstractions.TypeHandler;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SmartSql.Configuration
{
    public class TypeHandlerFactory
    {
        public static ITypeHandler Create(string typeString)
        {
            string typeName = typeString.Split(',')[0].Trim();
            string assemblyName = typeString.Split(',')[1].Trim();
            var assName = new AssemblyName { Name = assemblyName };
            Type typeHanderType = Assembly.Load(assName).GetType(typeName);
            return Activator.CreateInstance(typeHanderType) as ITypeHandler;
        }
    }
}
