using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.ConsoleTests
{
    public class SmartSqlGood
    {
        public string GoodJob(int a, long b, string c, decimal d, double e, float f, Guid g)
        {
            return $"{a}{b}{c}{d}{e}{f}{g}";
        }
    }
}
