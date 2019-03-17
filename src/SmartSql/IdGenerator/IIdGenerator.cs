namespace SmartSql.IdGenerator
{
    public interface IIdGenerator : IInitialize
    {
        long NextId();
    }
}