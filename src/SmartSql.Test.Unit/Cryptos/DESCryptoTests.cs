using System.Collections.Generic;
using FluentAssertions;
using SmartSql.TypeHandler.Crypto;
using Xunit;

namespace SmartSql.Test.Unit.Cryptos;

public class DESCryptoTests
{
    private readonly Dictionary<string, object> _defaultConfig = new Dictionary<string, object>
    {
        {"Key", "qxMfZpmQ1Rk="},
        {"IV", "XaX73vwx694="}
    };

    [Fact]
    public void Should_RoundTrip_When_EncryptAndDecrypt()
    {
        using var crypto = new DESCrypto();
        crypto.Initialize(_defaultConfig);
        var plainText = "SmartSql";

        var cipherText = crypto.Encrypt(plainText);
        var decryptText = crypto.Decrypt(cipherText);

        decryptText.Should().Be(plainText);
    }
}
