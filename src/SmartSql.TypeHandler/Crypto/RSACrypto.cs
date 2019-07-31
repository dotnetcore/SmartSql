using System.Collections.Generic;
using System.Security.Cryptography;

namespace SmartSql.TypeHandler.Crypto
{
    /// <summary>
    /// TODO impl
    /// </summary>
    public class RSACrypto : ICrypto
    {
        private const string PUBLIC_KEY = "PublicKey";
        private const string PRIVATE_KEY = "PrivateKey";

        public void Initialize(IDictionary<string, object> parameters)
        {
            parameters.EnsureValue(PUBLIC_KEY, out string publicKey);
            parameters.EnsureValue(PRIVATE_KEY, out string privateKey);
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
        public string Decrypt(string cipherText)
        {
            throw new System.NotImplementedException();
        }

        public string Encrypt(string plainText)
        {
            throw new System.NotImplementedException();
        }
    }
}