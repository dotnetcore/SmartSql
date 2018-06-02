using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Text;

namespace SmartSql.Configuration
{
    public class DbProviderFactoryFactory
    {
        public static DbProviderFactory Create(String typeString)
        {
            string typeName = typeString.Split(',')[0].Trim();
            string assemblyName = typeString.Split(',')[1].Trim();
            return Assembly.Load(new AssemblyName { Name = assemblyName })
                               .GetType(typeName)
                               .GetField("Instance")
                               .GetValue(null) as DbProviderFactory;
        }
    }
}
