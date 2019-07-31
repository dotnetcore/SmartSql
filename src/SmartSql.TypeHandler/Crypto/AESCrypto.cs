using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace SmartSql.TypeHandler.Crypto
{
    public class AESCrypto : ICrypto
    {
        private const string KEY = "Key";
        private const string IV = "IV";
        private Aes aesAlg;

        public void Initialize(IDictionary<string, object> parameters)
        {
            parameters.EnsureValue(KEY, out string key);
            parameters.EnsureValue(IV, out string iv);

            aesAlg = Aes.Create();
            aesAlg.Key = Convert.FromBase64String(key);
            aesAlg.IV = Convert.FromBase64String(iv);
        }

        public void Dispose()
        {
            aesAlg.Dispose();
        }

        public string Decrypt(string cipherText)
        {
            var cipherBytes = Convert.FromBase64String(cipherText);
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }

        public string Encrypt(string plainText)
        {
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }

                    var cipherBytes = msEncrypt.ToArray();
                    return Convert.ToBase64String(cipherBytes);
                }
            }
        }
    }
}