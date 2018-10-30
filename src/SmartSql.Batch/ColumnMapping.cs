using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Batch
{
    public class ColumnMapping
    {
        public string Column { get; set; }
        public string Mapping { get; set; }
        public string DataTypeName { get; set; }
        public int Ordinal { get; set; }
    }
}
