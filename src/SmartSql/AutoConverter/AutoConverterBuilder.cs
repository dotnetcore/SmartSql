using System;

namespace SmartSql.AutoConverter
{
    public class AutoConverterBuilder : IAutoConverterBuilder
    {
        public IAutoConverter Build(String name, IWordsConverter wordsConverter, ITokenizer tokenizer)
        {
            return new AutoConverter(name, tokenizer, wordsConverter);
        }
    }
}