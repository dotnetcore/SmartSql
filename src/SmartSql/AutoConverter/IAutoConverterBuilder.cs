using System;

namespace SmartSql.AutoConverter
{
    public interface IAutoConverterBuilder
    {
        IAutoConverter Build(String name, IWordsConverter wordsConverter, ITokenizer tokenizer);
    }
}