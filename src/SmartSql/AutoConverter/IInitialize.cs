using System.Collections.Generic;

namespace SmartSql.AutoConverter
{
    public interface IInitialize
    {
        void Initialize(IDictionary<string, object> parameters);
    }
}