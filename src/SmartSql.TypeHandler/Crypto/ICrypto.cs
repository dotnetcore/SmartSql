using System;

namespace SmartSql.TypeHandler.Crypto
{
    public interface ICrypto : IInitialize, IDisposable
    {
        string Decrypt(string cipherText);
        string Encrypt(string plainText);
    }
}