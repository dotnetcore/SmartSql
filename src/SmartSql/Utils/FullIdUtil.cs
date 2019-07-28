using System;

namespace SmartSql.Utils
{
    public class FullIdUtil
    {
        public static ValueTuple<String, String> Parse(string fullId)
        {
            if (String.IsNullOrEmpty(fullId))
            {
                throw new ArgumentNullException(nameof(fullId));
            }

            var ids = fullId.Split('.');
            if (ids.Length!=2)
            {
                throw new ArgumentException(nameof(fullId));
            }

            return (ids[0], ids[1]);
        }
    }
}