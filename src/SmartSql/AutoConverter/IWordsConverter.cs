using System;
using System.Collections.Generic;

namespace SmartSql.AutoConverter
{
    public interface IWordsConverter : IInitialize
    {
        String Convert(IEnumerable<String> words);
    }
}