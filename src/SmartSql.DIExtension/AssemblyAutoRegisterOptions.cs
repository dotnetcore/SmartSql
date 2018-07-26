using SmartSql.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.DIExtension
{
    public class AssemblyAutoRegisterOptions
    {
        public Func<IServiceProvider, ISmartSqlMapper> GetSmartSql { get; set; }

        public string AssemblyString { get; set; }
        public string ScopeTemplate { get; set; }
        public Func<Type, bool> Filter { get; set; }

        public void UseTypeFilter<T>()
        {
            Filter = (type) =>
            {
                return typeof(T).IsAssignableFrom(type);
            };
        }
    }
}
