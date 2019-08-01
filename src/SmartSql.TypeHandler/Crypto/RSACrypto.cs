using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SmartSql.TypeHandler.Crypto
{
    /// <summary>
    /// TODO impl
    /// </summary>
    public class RSACrypto : ICrypto
    {
        private const string PUBLIC_KEY = "PublicKey";
        private const string PRIVATE_KEY = "PrivateKey";
        private RSA _privateKeyProvider;
        private RSA _publicKeyProvider;
        private readonly Encoding _encoding = Encoding.UTF8;

        public void Initialize(IDictionary<string, object> parameters)
        {
            parameters.EnsureValue(CryptoFactory.ALGORITHM, out string algStr);
            parameters.EnsureValue(PUBLIC_KEY, out string publicKey);
            parameters.EnsureValue(PRIVATE_KEY, out string privateKey);
            _privateKeyProvider = RSA.Create();
            //_privateKeyProvider.ImportParameters();
            _publicKeyProvider = RSA.Create();
            //_publicKeyProvider.ImportParameters();
        }

        public void Dispose()
        {
            _privateKeyProvider?.Dispose();
            _publicKeyProvider?.Dispose();
        }

        public string Decrypt(string cipherText)
        {
            var cipherBytes = Convert.FromBase64String(cipherText);
            var plainBytes = _privateKeyProvider.Decrypt(cipherBytes, RSAEncryptionPadding.Pkcs1);
            return _encoding.GetString(plainBytes);
        }

        public string Encrypt(string plainText)
        {
            var plainBytes = _encoding.GetBytes(plainText);
            var cipherBytes = _publicKeyProvider.Encrypt(plainBytes, RSAEncryptionPadding.Pkcs1);
            return Convert.ToBase64String(cipherBytes);
        }
    }
}