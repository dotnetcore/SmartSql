using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Exceptions
{
    public class SmartSqlException : Exception
    {
        public SmartSqlException() : base("SmartSql throw an exception.") { }
        public SmartSqlException(Exception ex) : base("SmartSql throw an exception.", ex) { }
        public SmartSqlException(string message) : base(message) { }
        public SmartSqlException(string message, Exception inner) : base(message, inner) { }

    }
}
