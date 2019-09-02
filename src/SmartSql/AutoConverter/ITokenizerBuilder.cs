using System;
using System.Collections.Generic;

namespace SmartSql.AutoConverter
{
    public interface ITokenizerBuilder
    {
        ITokenizer Build(String name, IDictionary<String, object> properties);
    }
}