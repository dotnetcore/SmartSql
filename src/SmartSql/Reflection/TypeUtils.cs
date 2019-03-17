using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SmartSql.Reflection
{
    public class TypeUtils
    {
        public static Type GetType(string typeString)
        {
            string[] typeStrings = typeString.Split(',');
            string typeName = typeStrings[0].Trim();
            string assemblyName = typeStrings[1].Trim();
            var assName = new AssemblyName { Name = assemblyName };
            return Assembly.Load(assName).GetType(typeName);
        }
        
    }
}
