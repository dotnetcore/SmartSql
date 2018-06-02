using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartSql.Utils
{
    /// <summary>
    /// 权重筛选器
    /// </summary>
    public class WeightFilter<T>
    {
        /// <summary>
        /// 选举权重源
        /// </summary>
        /// <param name="inWeightSources"></param>
        /// <returns></returns>
        public WeightSource Elect(IEnumerable<WeightSource> inWeightSources)
        {
            var random = new Random();
            int totalWeight = inWeightSources.Sum(source => source.Weight);
            int position = random.Next(1, totalWeight);
            var sel_source = FindByPosition(inWeightSources, position);
            return sel_source;
        }

        private WeightSource FindByPosition(IEnumerable<WeightSource> sources, int position)
        {
            int startPosition = 0;
            foreach (var source in sources)
            {
                int endPosition = source.Weight + startPosition;
                if (position > startPosition && position <= endPosition)
                {
                    return source;
                }
                startPosition += source.Weight;
            }
            return null;
        }
        /// <summary>
        /// 权重源
        /// </summary>
        public class WeightSource
        {
            public T Source { get; set; }
            public int Weight { get; set; }
        }
    }
}
