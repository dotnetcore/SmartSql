using SmartSql.Abstractions.TypeHandler;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql
{
    public class DbParameter
    {
        public DbParameter()
        {

        }
        public DbParameter(string name, object val)
        {
            this.Name = name;
            this.Value = val;
        }
        public string Name { get; set; }
        public object Value { get; set; }
        public IDbDataParameter SourceParameter { get; internal set; }
        public byte? Precision { get; set; }
        public byte? Scale { get; set; }
        public int? Size { get; set; }
        public DbType? DbType { get; set; }
        public ParameterDirection? Direction { get; set; }
        public ITypeHandler TypeHandler { get; set; }
    }
}
