using System.Collections.Generic;
using SmartSql.Exceptions;

namespace SmartSql.AutoConverter
{
    public class WordsConverterBuilder : IWordsConverterBuilder
    {
        public IWordsConverter Build(string name, IDictionary<string, object> properties)
        {
            IWordsConverter wordsConverter;
            switch (name.ToUpper())
            {
                case "CAMEL":
                    wordsConverter = new CamelCaseConverter();
                    break;
                case "DELIMITER":
                    wordsConverter = new DelimiterConverter();
                    break;
                case "PASCAL":
                    wordsConverter = new PascalCaseConverter();
                    break;
                case "PASCALSINGULAR":
                    wordsConverter = new PascalCaseSingularConverter();
                    break;
                case "STRIKETHROUGH":
                    wordsConverter = new StrikeThroughConverter();
                    break;
                case "NONE":
                    wordsConverter = new NoneConverter();
                    break;
                default:
                    throw new SmartSqlException($"WordsConverter.Name -> {name} can not found");
            }

            wordsConverter.Initialize(properties);
            return wordsConverter;
        }
    }
}