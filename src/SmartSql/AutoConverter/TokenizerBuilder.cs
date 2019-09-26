using System.Collections.Generic;
using SmartSql.Exceptions;

namespace SmartSql.AutoConverter
{
    public class TokenizerBuilder : ITokenizerBuilder
    {
        public ITokenizer Build(string name, IDictionary<string, object> properties)
        {
            switch (name.ToUpper())
            {
                case "DEFAULT":
                    ITokenizer tokenizer = new DefaultTokenizer();
                    tokenizer.Initialize(properties);
                    return tokenizer;
                case "NONE":
                    return new NoneTokenizer();
                default:
                    throw new SmartSqlException($"Tokenizer.Name -> {name} can not found");
            }
        }
    }
}