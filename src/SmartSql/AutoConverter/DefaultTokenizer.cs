using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SmartSql.AutoConverter
{
    /// <summary>
    /// 默认分词器
    /// </summary>
    public class DefaultTokenizer : ITokenizer
    {
        private readonly Regex _uppercaseSplit = new Regex("(?=[A-Z])");
        public const string IGNORE_PREFIX_KEY = nameof(IgnorePrefix);
        public const string DELIMITER_KEY = nameof(Delimiter);
        public const string UPPERCASESPLIT_KEY = nameof(UppercaseSplit);
        /// <summary>
        /// 忽略前缀
        /// </summary>
        public String IgnorePrefix { get; set; }
        /// <summary>
        /// 分隔符
        /// </summary>
        public String Delimiter { get; set; }
        /// <summary>
        /// 开启大写字符分割
        /// </summary>
        public bool UppercaseSplit { get; set; } = true;

        public bool Initialized { get; private set; }

        public string Name => "Default";

        public IEnumerable<string> Segment(string phrase)
        {
            if (!String.IsNullOrEmpty(IgnorePrefix) && phrase.StartsWith(IgnorePrefix))
            {
                phrase = phrase.Substring(IgnorePrefix.Length);
            }
            #region UppercaseSplit
            if (UppercaseSplit)
            {
                var splitWords = _uppercaseSplit.Split(phrase).Where(m => !String.IsNullOrEmpty(m));
                if (String.IsNullOrEmpty(Delimiter))
                {
                    return splitWords;
                }
                else
                {
                    phrase = String.Join(Delimiter, splitWords);
                }
            }
            #endregion
            #region Delimiter
            if (!String.IsNullOrEmpty(Delimiter))
            {
                return phrase.Split(Delimiter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            }
            #endregion
            return new string[] { phrase };
        }

        public void Initialize(IDictionary<string, object> parameters)
        {
            if (parameters != null)
            {
                if (parameters.Value(IGNORE_PREFIX_KEY, out string ignorePre))
                {
                    IgnorePrefix = ignorePre;
                }
                if (parameters.Value(DELIMITER_KEY, out string delimiter))
                {
                    Delimiter = delimiter;
                }
                if (parameters.Value(UPPERCASESPLIT_KEY, out bool upperSplit))
                {
                    UppercaseSplit = upperSplit;
                }
            }
            Initialized = true;
        }
    }
}
