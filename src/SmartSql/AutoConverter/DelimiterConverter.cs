using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartSql.Exceptions;

namespace SmartSql.AutoConverter
{
    public class DelimiterConverter : IWordsConverter
    {
        private const string PREFIX = "Prefix";
        private const string DELIMITER = "Delimiter";
        private const string CONVERT_MODE = "Mode";
        private string _prefix = string.Empty;
        private string _delimiter = "_";
        private ConvertMode _convertMode = ConvertMode.None;

        public bool Initialized { get; private set; }

        public string Name => "Delimiter";

        public string Convert(IEnumerable<string> words)
        {
            var phrase = String.Empty;
            switch (_convertMode)
            {
                case ConvertMode.AllLower:
                    {
                        phrase = String.Join(_delimiter, words).ToLower();
                        break;
                    }
                case ConvertMode.AllUpper:
                    {
                        phrase = String.Join(_delimiter, words).ToUpper(); break;
                    }
                case ConvertMode.FirstUpper:
                    {
                        var firstUpperWords = words.Select(word =>
                          {
                              string firstChar = word.Substring(0, 1).ToUpper();
                              string leftChar = word.Substring(1).ToLower();
                              return firstChar + leftChar;
                          });
                        phrase = String.Join(_delimiter, firstUpperWords); break;
                    }
                case ConvertMode.None:
                    {
                        phrase = String.Join(_delimiter, words); break;
                    }
                default:
                    {
                        throw new SmartSqlException($"can not support ConvertMode:{_convertMode}");
                    }
            }
            return _prefix + phrase;
        }

        public void Initialize(IDictionary<string, object> parameters)
        {
            if (parameters != null)
            {
                if (!parameters.Value(DELIMITER, out _delimiter))
                {
                    _delimiter = "_";
                }
                if (!parameters.Value(CONVERT_MODE, out _convertMode))
                {
                    _convertMode = ConvertMode.None;
                }
                if (!parameters.Value(PREFIX, out _prefix))
                {
                    _prefix = string.Empty;
                }
            }
            Initialized = true;
        }

        public enum ConvertMode
        {
            None,
            AllLower,
            AllUpper,
            FirstUpper
        }
    }

}
