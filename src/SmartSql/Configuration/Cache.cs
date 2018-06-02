using SmartSql.Abstractions.Cache;
using SmartSql.Cache;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace SmartSql.Configuration
{
    public class Cache
    {
        public String Id { get; set; }
        public String Type { get; set; }
        public String TypeName { get { return Type.Split(',')[0]; } }
        public String AssemblyName { get { return Type.Split(',')[1]; } }
        public IDictionary Parameters { get; set; }
        public IList<FlushOnExecute> FlushOnExecutes { get; set; }
        public FlushInterval FlushInterval { get; set; }
        public ICacheProvider Provider { get; internal set; }

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
