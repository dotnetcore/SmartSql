using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.IdGenerator
{
    public interface IIdGeneratorBuilder
    {
        IIdGenerator Build(string type, IDictionary<string, object> parameters);
    }
}
