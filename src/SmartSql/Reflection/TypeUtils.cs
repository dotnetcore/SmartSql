using System;
using System.Reflection;
using SmartSql.Exceptions;

namespace SmartSql.Reflection
{
    public class TypeUtils
    {
        public static Type GetType(string typeString)
        {
            string[] typeStrings = typeString.Split(',');
            if (typeStrings.Length != 2)
            {
                throw new SmartSqlException($"Illegal parameter for typeString:[{typeString}].");
            }
            string typeName = typeStrings[0].Trim();
            string assemblyName = typeStrings[1].Trim();
            var assName = new AssemblyName {Name = assemblyName};
            var type = Assembly.Load(assName).GetType(typeName);
            if (type == null)
            {
                throw new SmartSqlException($"can not find Type:[{typeString}].");
            }

            return type;
        }
    }
}