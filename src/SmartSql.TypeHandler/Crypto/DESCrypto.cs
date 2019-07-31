using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace SmartSql.TypeHandler.Crypto
{
    public class DESCrypto : ICrypto
    {
        private const string KEY = "Key";
        private const string IV = "IV";
        private DES _desAlg;

        public void Initialize(IDictionary<string, object> parameters)
        {
            parameters.EnsureValue(KEY, out string key);
            parameters.EnsureValue(IV, out string iv);

            _desAlg = DES.Create();
            _desAlg.Key = Convert.FromBase64String(key);
            _desAlg.IV = Convert.FromBase64String(iv);
        }

        public void Dispose()
        {
            _desAlg.Dispose();
        }

        public string Decrypt(string cipherText)
        {
            var cipherBytes = Convert.FromBase64String(cipherText);
            ICryptoTransform decryptor = _desAlg.CreateDecryptor(_desAlg.Key, _desAlg.IV);

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
            ICryptoTransform encryptor = _desAlg.CreateEncryptor(_desAlg.Key, _desAlg.IV);

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