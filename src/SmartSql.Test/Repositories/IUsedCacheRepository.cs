using System;
using SmartSql.DyRepository.Annotations;
using SmartSql.Test.Entities;

namespace SmartSql.Test.Repositories
{
    [Cache("DateCache", "Lru", FlushInterval = 6000)]
    [Cache("LruCache", "Lru", FlushInterval = 6000)]
    [Cache("UserCache", "Fifo", FlushOnExecutes = new[] {"Insert", "UpdateUserName"})]
    public interface IUsedCacheRepository
    {
        [ResultCache("DateCache", Key = "GetNow")]
        [Statement(Sql = "Select GetDate();")]
        DateTime GetNow();

        [Statement(Sql = "INSERT INTO [T_User] ([UserName],[Status]) VALUES (@UserName,@Status);;Select Scope_Identity();")]
        long Insert(User user);

        [ResultCache("LruCache", Key = "GetId:$id")]
        [Statement(Sql = "Select @id;")]
        int GetId(long id);

        [ResultCache("UserCache", Key = "GetUserById:$id")]
        [Statement(Sql = "select * from T_User where id=@id;")]
        User GetUserById(long id);

        [Statement(Sql = "update T_User set UserName=@userName where id=@id;")]
        int UpdateUserName(long id, string userName);
    }
}