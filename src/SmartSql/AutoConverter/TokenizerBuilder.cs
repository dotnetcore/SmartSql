using System.Collections.Generic;
using SmartSql.Exceptions;

namespace SmartSql.AutoConverter
{
    public class TokenizerBuilder : ITokenizerBuilder
    {
        public ITokenizer Build(string name, IDictionary<string, object> properties)
        {
            switch (name)
            {
                case "Default":
                    ITokenizer tokenizer = new DefaultTokenizer();
                    tokenizer.Initialize(properties);
                    return tokenizer;
               default:
                   throw new SmartSqlException($"Tokenizer.Name -> {name} can not found");
            }
        }
    }
}