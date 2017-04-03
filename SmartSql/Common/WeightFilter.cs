using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartSql.Common
{
    /// <summary>
    /// 权重筛选器
    /// </summary>
    public class WeightFilter<T>
    {
        public IList<Seed> Calculate(IList<Seed> inSeeds, int outCount)
        {
            if (inSeeds.Count <= outCount)
            {
                return inSeeds;
            }
            var outSeeds = new List<Seed>();
            var random = new Random();
            for (int i = 0; i < outCount; i++)
            {
                int totalWeight = inSeeds.Sum(seed => seed.Weight);
                int position = random.Next(1, totalWeight);
                var sel_seed = FindByPosition(inSeeds, position);
                if (sel_seed == null)
                {
                    continue;
                }
                outSeeds.Add(sel_seed);
                inSeeds.Remove(sel_seed);
            }
            return outSeeds;
        }

        private Seed FindByPosition(IList<Seed> seeds, int position)
        {
            int startPosition = 0;
           
            foreach (var seed in seeds)
            {
                int endPosition = seed.Weight + startPosition;
                if (position > startPosition && position <= endPosition)
                {
                    return seed;
                }
                startPosition += seed.Weight;
            }
            return null;
        }

        public class Seed
        {
            public T Source { get; set; }
            public int Weight { get; set; }
        }
    }
}
