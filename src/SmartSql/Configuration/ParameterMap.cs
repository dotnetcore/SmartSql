using SmartSql.TypeHandlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration
{
    [Obsolete]
    public class ParameterMap
    {
        public string Id { get; set; }
        /// <summary>
        /// Key:Property
        /// </summary>
        public IDictionary<String, Parameter> Parameters { get; set; }
    }
    [Obsolete]
    public class Parameter
    {
        public string Property { get; set; }
        public string Name { get; set; }
        public Type CSharpType { get; set; }
        public ITypeHandler Handler { get; set; }
    }
}
