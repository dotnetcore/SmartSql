using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SmartSql.Filters
{
    public class FilterCollection : Collection<IFilter>
    {
        public void Add<TFilter>()
            where TFilter : IFilter, new()
        {
            Add(new TFilter());
        }
    }
}