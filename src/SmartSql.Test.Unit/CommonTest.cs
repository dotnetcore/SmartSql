using System;
using Xunit;
using SmartSql.TypeHandlers;
using System.Reflection;
using System.Data;
using System.Dynamic;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using SmartSql.Test.Entities;
using SmartSql.Test.Repositories;

namespace SmartSql.Test.Unit
{
    public class CommonTest
    {
        [Fact]
        public void Test1()
        {
            Console.WriteLine("Test1");
        }
    }


}
