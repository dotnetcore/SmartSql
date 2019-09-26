using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.AutoConverter
{
    public class PascalCaseConverter : IWordsConverter
    {
        public bool Initialized => true;

        public string Name => "Pascal";

        public String Convert(IEnumerable<string> words)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var word in words)
            {
                string firstChar = word.Substring(0, 1).ToUpper();
                stringBuilder.Append(firstChar);
                string leftChar = word.Substring(1).ToLower();
                stringBuilder.Append(leftChar);
            }
            return stringBuilder.ToString();
        }

        public void Initialize(IDictionary<string, object> parameters)
        {
           
        }
    }
}
