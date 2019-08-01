using System;
using System.Collections.Generic;
using System.Reflection;

namespace SmartSql.DataConnector
{
    public class AppOptions
    {
        public IList<Task> Tasks { get; set; }

        public class Task
        {
            public String Path { get; set; }
        }
    }
}