using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SmartSql.Utils
{
    public class TypeScan
    {
        public static IList<Type> Scan(TypeScanOptions options)
        {
            var assembly = Assembly.Load(options.AssemblyString);
            return assembly.GetTypes().Where(options.Filter).ToList();
        }
    }

    public class TypeScanOptions
    {
        public TypeScanOptions()
        {
            Filter = (type) => true;
        }

        /// <summary>
        /// 仓储接口程序集
        /// </summary>
        public string AssemblyString { get; set; }

        /// <summary>
        /// 仓储接口筛选器
        /// </summary>
        public Func<Type, bool> Filter { get; set; }

        public void UseTypeFilter<T>()
        {
            Filter = (type) => typeof(T).IsAssignableFrom(type);
        }
    }
}