using System;
using System.Collections.Generic;

namespace SmartSql.TypeHandler.Crypto
{
    public class CryptoFactory
    {
        private const string ALGORITHM = "Algorithm";

        public static ICrypto Create(IDictionary<string, object> parameters)
        {
            if (!parameters.TryGetValue(ALGORITHM, out var alg))
            {
                throw new ArgumentNullException(ALGORITHM);
            }

            ICrypto crypto;
            switch (alg.ToString().ToUpper())
            {
                case "RSA":
                case "RSA2":
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