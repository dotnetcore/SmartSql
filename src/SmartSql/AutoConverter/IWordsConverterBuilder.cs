using System;
using System.Collections.Generic;

namespace SmartSql.AutoConverter
{
    public interface IWordsConverterBuilder
    {
        IWordsConverter Build(String name, IDictionary<String, object> properties);
    }
}