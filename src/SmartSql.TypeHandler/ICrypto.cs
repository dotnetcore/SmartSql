using System;

namespace SmartSql.TypeHandler
{
    public interface ICrypto : IInitialize, IDisposable
    {
        string Decrypt(string data);
        string Encrypt(string data);
    }
}