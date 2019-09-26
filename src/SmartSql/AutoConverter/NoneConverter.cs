using System;
using System.Collections.Generic;

namespace SmartSql.AutoConverter
{
    public class NoneConverter : IWordsConverter
    {
        public void Initialize(IDictionary<string, object> parameters)
        {
        }

        public string Convert(IEnumerable<string> words)
        {
            return String.Join("", words);
        }
    }
}