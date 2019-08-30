using System;
using System.Text.RegularExpressions;

namespace SmartSql.Utils
{
    public class NamingUtil
    {
        public static String CamelCase(string phrase)
        {
            string firstChar = phrase.Substring(0, 1).ToLower();
            return firstChar + phrase.Substring(1);
        }
        public static String PascalCase(string phrase)
        {
            string firstChar = phrase.Substring(0, 1).ToUpper();
            return firstChar + phrase.Substring(1);
        }

         /// <summary>讲单词转换为单数的形式</summary>
        public static string ToSingular(string phrase)
        {
            Regex plural1 = new Regex("(?<keep>[^aeiou])ies$");
            Regex plural2 = new Regex("(?<keep>[aeiou]y)s$");
            Regex plural3 = new Regex("(?<keep>[sxzh])es$");
            Regex plural4 = new Regex("(?<keep>[^sxzhyu])s$");
            if (plural1.IsMatch(phrase))
            {
                return plural1.Replace(phrase, "${keep}y");
            }
            else if (plural2.IsMatch(phrase))
            {
                return plural2.Replace(phrase, "${keep}");
            }
            else if (plural3.IsMatch(phrase))
            {
                return plural3.Replace(phrase, "${keep}");
            }
            else if (plural4.IsMatch(phrase))
            {
                return plural4.Replace(phrase, "${keep}");
            }
            return phrase;
        }

        /// <summary>讲单词转换为复数的形式</summary>
        public static string ToPlural(string phrase)
        {
            Regex plural1 = new Regex("(?<keep>[^aeiou])y$");
            Regex plural2 = new Regex("(?<keep>[aeiou]y)$");
            Regex plural3 = new Regex("(?<keep>[sxzh])$");
            Regex plural4 = new Regex("(?<keep>[^sxzhy])$");
            if (plural1.IsMatch(phrase))
            {
                return plural1.Replace(phrase, "${keep}ies");
            }
            else if (plural2.IsMatch(phrase))
            {
                return plural2.Replace(phrase, "${keep}s");
            }
            else if (plural3.IsMatch(phrase))
            {
                return plural3.Replace(phrase, "${keep}es");
            }
            else if (plural4.IsMatch(phrase))
            {
                return plural4.Replace(phrase, "${keep}s");
            }
            return phrase;
        }
    }
}