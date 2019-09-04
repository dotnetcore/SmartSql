using System;
using System.Collections.Generic;

namespace SmartSql.Options
{
    public class AutoConverterBuilder
    {
        public String Name { get; set; }

        public bool IsDefault { get; set; }

        public String TokenizerName { get; set; }

        public IDictionary<String, object> TokenizerProperties { get; set; }

        public String WordsConverterName { get; set; }

        public IDictionary<String, object> WordsConverterProperties { get; set; }
    }
}