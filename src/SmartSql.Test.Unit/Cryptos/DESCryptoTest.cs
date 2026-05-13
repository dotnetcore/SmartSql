using System.Collections.Generic;
using SmartSql.TypeHandler.Crypto;
using Xunit;

namespace SmartSql.Test.Unit.Cryptos
{
    public class DESCryptoTest
    {
        [Fact]
        public void Test()
        {
            using (var desCrypto = new DESCrypto())
            {
                desCrypto.Initialize(new Dictionary<string, object>
                {
                    {"Key", "qxMfZpmQ1Rk="},
                    {"IV", "XaX73vwx694="}
                });

                var plainText = "SmartSql";

                var cipherText = desCrypto.Encrypt(plainText);
                var decryptText = desCrypto.Decrypt(cipherText);
                Assert.Equal(plainText, decryptText);
            }
        }
    }
}