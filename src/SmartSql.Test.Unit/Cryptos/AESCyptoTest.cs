using System.Collections.Generic;
using SmartSql.TypeHandler.Crypto;
using Xunit;

namespace SmartSql.Test.Unit.Cryptos
{
    public class AESCyptoTest
    {
        [Fact]
        public void Test()
        {
            using (var aesCrypto = new AESCrypto())
            {
                aesCrypto.Initialize(new Dictionary<string, object>
                {
                    {"Key", "awVFRYPeTTrA9T7OOzaAFUvu8I/ZyYjAtIzEjCmzzYw="},
                    {"IV", "7cFxoI3/k1wxN9P6rEyR/Q=="}
                });
                var plainText = "SmartSql";

                var cipherText = aesCrypto.Encrypt(plainText);
                var decryptText = aesCrypto.Decrypt(cipherText);
                Assert.Equal(plainText, decryptText);
                cipherText = aesCrypto.Encrypt(plainText);
                decryptText = aesCrypto.Decrypt(cipherText);
                Assert.Equal(plainText, decryptText);
                cipherText = aesCrypto.Encrypt(plainText);
                decryptText = aesCrypto.Decrypt(cipherText);
                Assert.Equal(plainText, decryptText);
            }
        }
    }
}