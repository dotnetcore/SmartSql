using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Configuration;

namespace SmartSql.IdGenerator
{
    public class DbSequence : IIdGenerator
    {
        protected int Step { get; set; } = 1;
        protected long DbId { get; set; }
        protected long CurrentId { get; set; }

        protected String Sql { get; set; }
        private ISqlMapper _sqlMapper;
        public void Initialize(IDictionary<string, object> parameters)
        {
            if (parameters.Value(nameof(DbSequence.Step), out int step))
            {
                Step = step;
            }
        }

        public void SetupSmartSql(SmartSqlBuilder smartSqlBuilder)
        {
            _sqlMapper = smartSqlBuilder.SqlMapper;
        }

        public long NextId()
        {
            lock (this)
            {
                if (DbId != 0 && CurrentId < DbId + Step) return ++CurrentId;
                NextDbId();
                CurrentId = DbId;
                return CurrentId;
            }
        }

        private void NextDbId()
        {
            DbId = _sqlMapper.ExecuteScalar<long>(new RequestContext
            {
                RealSql = Sql
            });
        }
    }
}

