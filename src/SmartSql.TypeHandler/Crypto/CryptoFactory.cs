using System;
using System.Collections.Generic;

namespace SmartSql.TypeHandler.Crypto
{
    public class CryptoFactory
    {
        public const string ALGORITHM = "Algorithm";

        public static ICrypto Create(IDictionary<string, object> parameters)
        {
            parameters.EnsureValue(ALGORITHM, out String alg);

            ICrypto crypto;
            switch (alg.ToUpper())
            {
                case "RSA":
                {
                    crypto = new RSACrypto();
                    break;
                }

                case "DES":
                {
                    crypto = new DESCrypto();
                    break;
                }

                case "AES":
                {
                    crypto = new AESCrypto();
                    break;
                }

                default: throw new ArgumentException($"can not support Algorithm:[{alg}]", ALGORITHM);
            }

            crypto.Initialize(parameters);
            return crypto;
        }
    }
}