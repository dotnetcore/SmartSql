using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Configuration;

namespace SmartSql.IdGenerator
{
    public class DbSequence : IIdGenerator, ISetupSmartSql
    {
        private int _step;
        private string _sequenceSql;
        protected long DbId { get; set; }
        protected long MaxId { get; set; }
        protected long CurrentId { get; set; }
        private ISqlMapper _sqlMapper;
        public void Initialize(IDictionary<string, object> parameters)
        {
            parameters.EnsureValue("Step", out _step);
            parameters.EnsureValue("SequenceSql", out _sequenceSql);
        }
        public void SetupSmartSql(SmartSqlBuilder smartSqlBuilder)
        {
            _sqlMapper = smartSqlBuilder.SqlMapper;
        }

        public long NextId()
        {
            lock (this)
            {
                if (DbId != 0 && CurrentId < MaxId) return ++CurrentId;
                NextDbId();
                return CurrentId;
            }
        }

        private void NextDbId()
        {
            DbId = _sqlMapper.ExecuteScalar<long>(new RequestContext
            {
                RealSql = _sequenceSql
            });
            MaxId = DbId + _step - 1;
            CurrentId = DbId;
        }
    }
}

