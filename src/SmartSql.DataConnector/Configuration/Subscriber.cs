using System;
using System.Collections.Generic;
using SmartSql.InvokeSync;

namespace SmartSql.DataConnector.Configuration
{
    public class Subscriber
    {
        public String Type { get; set; }
        public Dictionary<String, String> Parameters { get; set; } = new Dictionary<String, String>();
        public ISubscriber Instance { get; set; }
    }
}