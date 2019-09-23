using System.Collections.Generic;

namespace SmartSql.AutoConverter
{
    public class NoneTokenizer : ITokenizer
    {
        public void Initialize(IDictionary<string, object> parameters)
        {
        }

        public IEnumerable<string> Segment(string phrase)
        {
            return new[] { phrase };
        }
    }
}