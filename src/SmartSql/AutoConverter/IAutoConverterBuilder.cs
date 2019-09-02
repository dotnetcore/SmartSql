namespace SmartSql.AutoConverter
{
    public interface IAutoConverterBuilder
    {
        IAutoConverter Build(IWordsConverter wordsConverter, ITokenizer tokenizer);
    }
}