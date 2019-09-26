using System.Collections.Generic;
using SmartSql.Utils;

namespace SmartSql.AutoConverter
{

    /// <summary>
    /// 在原来的 PascalCaseConverter 的基础之上， 增加了强制转成单数的转换， 应对某些数据
    /// 库表名称为复数单词的情况， 确认生成的类名是单数形式。（仅仅用正则表达式进行判断， 可能
    /// 会有误伤）。
    /// </summary>
    public class PascalCaseSingularConverter : IWordsConverter {

        private PascalCaseConverter converter = new PascalCaseConverter();

        public bool Initialized => converter.Initialized;

        public string Name => "PascalSingular";

        public string Convert(IEnumerable<string> words) {
            var pascalWords = converter.Convert(words);
            return NamingUtil.ToSingular(pascalWords);
        }

        public void Initialize(IDictionary<string, object> parameters) {
            converter.Initialize(parameters);
        }

    }

}
