using SmartSql.Abstractions.TypeHandler;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration.Maps
{
    public class ParameterMap
    {
        public string Id { get; set; }
        public IList<Parameter> Parameters { get; set; }
    }
    public class Parameter
    {
        public string Property { get; set; }
        public string Name { get; set; }
        public string TypeHandler { get; set; }
        public ITypeHandler Handler { get; set; }
    }
}
