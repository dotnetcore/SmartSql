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
        /// Key : PropertyName
        /// </summary>
        public SortedDictionary<String, ColumnAttribute> Columns { get; set; }
    }
}