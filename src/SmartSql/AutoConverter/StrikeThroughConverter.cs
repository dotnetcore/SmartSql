using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.AutoConverter {

    /// <summary>前端常用的小写中划线命名转换器</summary>
    public class StrikeThroughConverter : IWordsConverter {

        public bool Initialized { get; private set; }

        public string Name => "StrikeThrough";

        public string Convert(IEnumerable<string> words) {
            return string.Join("-", words).ToLowerInvariant();
        }

        public void Initialize(IDictionary<string, object> parameters) {
            Initialized = true;
        }
    }

}
