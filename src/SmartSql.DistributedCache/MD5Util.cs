using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.DistributedCache
{
    public class MD5Util
    {
        private static readonly string _defaultCharset = "utf-8";
        #region MD5
        public static string Encrypt(string data)
        {
            return Encrypt(data, "");
        }
        public static string Encrypt(string data, string privateKey)
        {
            return Encrypt(data, privateKey, _defaultCharset);
        }
        public static string Encrypt(string data, String privateKey, string charset)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                string encryptStr = data + privateKey;
                var encoding = Encoding.GetEncoding(charset);
                byte[] dataByte = md5.ComputeHash(encoding.GetBytes(encryptStr));
                var sb = new StringBuilder();
                for (int i = 0; i < dataByte.Length; i++)
                {
                    sb.Append(dataByte[i].ToString("x").PadLeft(2, '0'));
                }
                return sb.ToString();
            }
        }
        #endregion
    }
}
