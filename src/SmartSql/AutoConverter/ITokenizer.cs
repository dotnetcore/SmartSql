using System;
using System.Collections.Generic;

namespace SmartSql.AutoConverter
{
    public interface ITokenizer : IInitialize
    {
        IEnumerable<String> Segment(String phrase);
    }
}