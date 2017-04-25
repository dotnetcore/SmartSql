using SmartSql.Abstractions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace SmartSql.Logging
{
    public class ConsoleLogger : ILog
    {
        public void Debug(string message)
        {
            Console.WriteLine($"{DateTime.Now}:{message}");
        }

        public void Debug(string message, Exception exception)
        {
            Console.WriteLine($"{DateTime.Now}:{message}");
            Console.WriteLine($"{exception}");
        }

        public void Error(string message)
        {
            Console.WriteLine($"{DateTime.Now}:{message}");
        }

        public void Error(string message, Exception exception)
        {
            Console.WriteLine($"{DateTime.Now}:{message}");
            Console.WriteLine($"{exception}");
        }

        public void Fatal(string message)
        {
            Console.WriteLine($"{DateTime.Now}:{message}");
        }

        public void Fatal(string message, Exception exception)
        {
            Console.WriteLine($"{DateTime.Now}:{message}");
            Console.WriteLine($"{exception}");
        }

        public void Info(string message)
        {
            Console.WriteLine($"{DateTime.Now}:{message}");
        }

        public void Info(string message, Exception exception)
        {
            Console.WriteLine($"{DateTime.Now}:{message}");
            Console.WriteLine($"{exception}");
        }

        public void Warn(string message)
        {
            Console.WriteLine($"{DateTime.Now}:{message}");
        }

        public void Warn(string message, Exception exception)
        {
            Console.WriteLine($"{DateTime.Now}:{message}");
            Console.WriteLine($"{exception}");
        }
    }
}
