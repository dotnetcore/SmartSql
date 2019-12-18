using System.Collections.Generic;
using SmartSql.TypeHandler.Crypto;
using Xunit;

namespace SmartSql.Test.Unit.Cryptos
{
    public class RSACryptoTest
    {
        // [Fact]
        public void Test()
        {
            using (var rsaCrypto = new RSACrypto())
            {
                rsaCrypto.Initialize(new Dictionary<string, object>
                {
                    {"Algorithm", "RSA"},
                    {"PublicKey", "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQD2KXcCt30MrOOt4F1vhtfNViBGapFRmxaOnSVK9FMPV63Pr8Y3EzFpLFH58TzbNtwoB5aWRy4zjImbwsy5S2eqmSkJQWbWZiyxMLEYy1+dQDr27SihLp7BvrDUj8uy+LIO6fwCDMKJ2sQtXh+Uk6AO9meqdpA5riUEDVQKbpw6IQIDAQAB"},
                    {"PrivateKey", "MIICXQIBAAKBgQD2KXcCt30MrOOt4F1vhtfNViBGapFRmxaOnSVK9FMPV63Pr8Y3EzFpLFH58TzbNtwoB5aWRy4zjImbwsy5S2eqmSkJQWbWZiyxMLEYy1+dQDr27SihLp7BvrDUj8uy+LIO6fwCDMKJ2sQtXh+Uk6AO9meqdpA5riUEDVQKbpw6IQIDAQABAoGBAL0EW+kPIgtgmjdCaByiKwT11DSd0dYawzhg/GjQsRK/3avzKb3SlAdRS+UjUvp05poqMXxVTemxSVz8OJ0zhHYePa+wE0E31sBfnMxpOo5LIraTQuKGhE5IXNgUB0xYyAkG4vLLmoGDFCUHWPHYw5591C66eNyhJMSkPcqSCbMNAkEA+8HXhp3Ut1LVqBFnSpUIoRkUfXotWvfJjat6GGtmyglSRqTcUkVwIlBabVEMDyoMS9/LQcXM2QQ3bJiTKpDY3wJBAPpPe8Tfjm2o1Oxz8aGjnt+u2b4SZ9WXXkbZY1qsMNAkrP7ZFF5MTIm9L18V+yrvZ0mU7YC5Mck+F5eEbIxOTP8CQQDf0X1i2H58XNBGEvLZg5WgY0OsKiqYbSJrKL/rZdCEXbUfyQF2wvTmDLnX5e3qrV8xNUzmtIthhDYh/aMYfJ3RAkAMy39SIvNO27B2nb6eOpTmbjOnKZ2xJ1mkWXxgqCiemyFUrZgC8fd/mvIO9DqwiShIdJpnWBAZb1kZX6WEzoPVAkAz0CHiCH5JGdf71zCifbfFfcXA9+IXqQxvU9+3yGLAQW1S7xd94f8uOX+CsWsUchOpwmgzXYpvMLEtu822WqfF"}
                });
                var plainText = "SmartSql";

                var cipherText = rsaCrypto.Encrypt(plainText);
                var decryptText = rsaCrypto.Decrypt(cipherText);
                Assert.Equal(plainText, decryptText);
            }
        }
    }
}