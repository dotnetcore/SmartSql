using SmartSql.Annotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.CUD
{
    public class EntityMetaData
    {
        public string TableName { get; set; }
        public ColumnAttribute PrimaryKey { get; set; }
        /// <summary>
        /// Key :ColumnName
        /// </summary>
        public IDictionary<String, ColumnAttribute> Columns { get; set; }
    }
}
