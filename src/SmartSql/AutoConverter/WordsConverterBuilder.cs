using System.Collections.Generic;
using SmartSql.Exceptions;

namespace SmartSql.AutoConverter
{
    public class WordsConverterBuilder : IWordsConverterBuilder
    {
        public IWordsConverter Build(string name, IDictionary<string, object> properties)
        {
            IWordsConverter wordsConverter;
            switch (name)
            {
                case "Camel":
                    wordsConverter = new CamelCaseConverter();
                    break;
                case "Delimiter":
                    wordsConverter = new DelimiterConverter();
                    break;
                case "Pascal":
                    wordsConverter = new PascalCaseConverter();
                    break;
                case "PascalSingular":
                    wordsConverter = new PascalCaseSingularConverter();
                    break;
                case "StrikeThrough":
                    wordsConverter = new StrikeThroughConverter();
                    break;
                default:
                    throw new SmartSqlException($"WordsConverter.Name -> {name} can not found");
            }

            wordsConverter.Initialize(properties);
            return wordsConverter;
        }
    }
}