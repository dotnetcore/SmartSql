using System.Collections.Generic;
using SmartSql.TypeHandler.Crypto;
using Xunit;

namespace SmartSql.Test.Unit.Cryptos
{
    public class RSACryptoTest
    {
        [Fact]
        public void RSA()
        {
            using (var rsaCrypto = new RSACrypto())
            {
                rsaCrypto.Initialize(new Dictionary<string, object>
                {
                    {"Algorithm", "RSA"},
                    {"PublicKey", ""},
                    {"PrivateKey", ""}
                });
                var plainText = "SmartSql";

                var cipherText = rsaCrypto.Encrypt(plainText);
                var decryptText = rsaCrypto.Decrypt(cipherText);
                Assert.Equal(plainText, decryptText);
            }
        }

        [Fact]
        public void RSA2()
        {
            using (var rsaCrypto = new RSACrypto())
            {
                rsaCrypto.Initialize(new Dictionary<string, object>
                {
                    {"Algorithm", "RSA2"},
                    {"PublicKey", ""},
                    {"PrivateKey", ""}
                });
                var plainText = "SmartSql";

                var cipherText = rsaCrypto.Encrypt(plainText);
                var decryptText = rsaCrypto.Decrypt(cipherText);
                Assert.Equal(plainText, decryptText);
            }
        }
    }
}