using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Configuration;

namespace SmartSql.IdGenerator
{
    public class DbSequence : IIdGenerator
    {
        private SmartSqlConfig _smartSqlConfig;
        
        protected int Step { get; set; } = 1;
        protected int DbId { get; set; } 
        protected int CurrentId { get; set; } = 1;
        protected String Sql { get; set; }

        public void Initialize(IDictionary<string, object> parameters)
        {
            if (parameters.Value(nameof(DbSequence.Step), out int step))
            {
                Step = step;
            }
            parameters.EnsureValue(nameof(SmartSqlConfig), out _smartSqlConfig);
        }

        public long NextId()
        {
            throw new NotImplementedException();
        }
    }
}
