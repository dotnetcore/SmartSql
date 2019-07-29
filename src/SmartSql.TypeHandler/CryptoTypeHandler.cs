using System;
using System.Collections.Generic;
using SmartSql.Data;
using SmartSql.TypeHandler.Crypto;
using SmartSql.TypeHandlers;

namespace SmartSql.TypeHandler
{
    public class CryptoTypeHandler : AbstractNullableTypeHandler<String, String>
    {
        private ICrypto _crypto;

        protected override string GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            var strVal = dataReader.GetString(columnIndex);
            return _crypto.Decrypt(strVal);
        }

        protected override object GetSetParameterValueWhenNotNull(object parameterValue)
        {
            return _crypto.Encrypt(parameterValue.ToString());
        }

        public override void Initialize(IDictionary<string, object> parameters)
        {
            _crypto = CryptoFactory.Create(parameters);
        }
    }
}