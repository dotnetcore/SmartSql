using System.Collections.Generic;
using FluentAssertions;
using SmartSql.TypeHandler.Crypto;
using Xunit;

namespace SmartSql.Test.Unit.Cryptos;

public class AESCryptoTests
{
    private readonly Dictionary<string, object> _defaultConfig = new Dictionary<string, object>
    {
        {"Key", "awVFRYPeTTrA9T7OOzaAFUvu8I/ZyYjAtIzEjCmzzYw="},
        {"IV", "7cFxoI3/k1wxN9P6rEyR/Q=="}
    };

    [Fact]
    public void Should_RoundTrip_When_EncryptAndDecrypt()
    {
        using var crypto = new AESCrypto();
        crypto.Initialize(_defaultConfig);
        var plainText = "SmartSql";

        var cipherText = crypto.Encrypt(plainText);
        var decryptText = crypto.Decrypt(cipherText);

        decryptText.Should().Be(plainText);
    }

    [Fact]
    public void Should_RoundTrip_When_EncryptAndDecryptMultipleTimes()
    {
        using var crypto = new AESCrypto();
        crypto.Initialize(_defaultConfig);
        var plainText = "SmartSql";

        for (int i = 0; i < 3; i++)
        {
            var cipherText = crypto.Encrypt(plainText);
            var decryptText = crypto.Decrypt(cipherText);
            decryptText.Should().Be(plainText);
        }
    }

    [Fact]
    public void Should_RoundTrip_When_LargePlainText()
    {
        using var crypto = new AESCrypto();
        crypto.Initialize(_defaultConfig);
        var plainText = new string('A', 10000);

        var cipherText = crypto.Encrypt(plainText);
        var decryptText = crypto.Decrypt(cipherText);

        decryptText.Should().Be(plainText);
    }
}
