using SmartSql.Cache;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SmartSql.Configuration
{
    public class Cache
    {
        public String Id { get; set; }
        public String Type { get; set; }
        public IDictionary<String, Object> Parameters { get; set; }
        public IList<FlushOnExecute> FlushOnExecutes { get; set; }
        public FlushInterval FlushInterval { get; set; }
        public ICacheProvider Provider { get; set; }
    }

    public class FlushInterval
    {
        public TimeSpan Interval => new TimeSpan(Hours, Minutes, Seconds);

        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
    }

    public class FlushOnExecute
    {
        public String Statement { get; set; }
    }
}