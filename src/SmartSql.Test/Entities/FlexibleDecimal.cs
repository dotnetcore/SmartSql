using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Test.Entities
{
    public class FlexibleDecimal
    {
        public Decimal Decimal { get; set; }
        public Decimal Boolean { get; set; }
        public Decimal Char { get; set; }
        public Decimal Byte { get; set; }
        public Decimal Int16 { get; set; }
        public Decimal Int32 { get; set; }
        public Decimal Int64 { get; set; }
        public Decimal String { get; set; }
    }
}
