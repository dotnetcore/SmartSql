using System;

namespace SmartSql.AutoConverter
{
    public class AutoConverter : IAutoConverter
    {
        private readonly ITokenizer _tokenizer;
        private readonly IWordsConverter _wordsConverter;

        public AutoConverter(String name, ITokenizer tokenizer, IWordsConverter wordsConverter)
        {
            Name = name;
            _tokenizer = tokenizer;
            _wordsConverter = wordsConverter;
        }

        public string Name { get; }

        public string Convert(string input)
        {
            return _wordsConverter.Convert(_tokenizer.Segment(input));
        }
    }
}