using SmartSql.Configuration.Tags;
using SmartSql.DataSource;
using System;
using System.Collections.Generic;
using System.Data;
using SmartSql.AutoConverter;

namespace SmartSql.Configuration
{
    public class Statement
    {
        public SqlMap SqlMap { get; set; }
        public String Id { get; set; }
        public StatementType StatementType { get; set; } = StatementType.Unknown;
        public CommandType? CommandType { get; set; }
        public DataSourceChoice? SourceChoice { get; set; }
        public IsolationLevel? Transaction { get; set; }
        public bool EnablePropertyChangedTrack { get; set; }
        public String ReadDb { get; set; }
        public int? CommandTimeout { get; set; }
        public String FullSqlId => $"{SqlMap.Scope}.{Id}";
        public IList<ITag> SqlTags { get; set; }

        public IAutoConverter AutoConverter { get; set; }
        
        #region Map

        public String CacheId { get; set; }
        public Cache Cache { get; set; }
        public String ParameterMapId { get; set; }
        public ParameterMap ParameterMap { get; set; }
        public String ResultMapId { get; set; }
        public ResultMap ResultMap { get; set; }
        public String MultipleResultMapId { get; set; }
        public MultipleResultMap MultipleResultMap { get; set; }

        #endregion

        internal IList<Include> IncludeDependencies { get; set; }

        public void BuildSql(AbstractRequestContext context)
        {
            foreach (ITag tag in SqlTags)
            {
                tag.BuildSql(context);
            }
        }
    }
}