using SmartSql.Abstractions.TypeHandler;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration.Maps
{
    public class ResultMap
    {
        public string Id { get; set; }
        public Constructor Constructor { get; set; }
        public IList<Result> Results { get; set; }
    }

    public class Constructor
    {
        public IList<Arg> Args { get; set; }
    }

    public class Arg
    {
        public string Column { get; set; }
        public string Type { get; set; }
        public Type ArgType { get; set; }
        public string TypeHandler { get; set; }
        public ITypeHandler Handler { get; set; }
    }

    public class Result
    {
        public string Property { get; set; }
        public string Column { get; set; }
        public string TypeHandler { get; set; }
        public ITypeHandler Handler { get; set; }
    }
}
