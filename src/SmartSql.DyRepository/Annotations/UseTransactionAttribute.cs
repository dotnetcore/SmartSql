using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.DyRepository.Annotations
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class UseTransactionAttribute : Attribute
    {
        public IsolationLevel Level { get; set; } = IsolationLevel.Unspecified;
    }
}
