using System;
using System.Reflection;
using SmartSql.InvokeSync;

namespace SmartSql.DataConnector.Configuration
{
    public class Job
    {
        public Source Source { get; set; }
        public Destination Dest { get; set; }
    }
}