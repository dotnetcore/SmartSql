using SmartSql.Abstractions;
using SmartSql.Abstractions.Cache;
using SmartSql.Cache;
using SmartSql.Configuration.Tags;
using SmartSql.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace SmartSql.Configuration.Statements
{
    public enum StatementType
    {
        Statement,
        Insert,
        Update,
        Delete,
        Select
    }

    public class Statement
    {
        public virtual StatementType Type { get { return StatementType.Statement; } }
        [XmlIgnore]
        public SmartSqlMap SmartSqlMap { get; internal set; }
        [XmlAttribute]
        public String Id { get; set; }
        public String FullSqlId => $"{SmartSqlMap.Scope}.{Id}";
        public List<ITag> SqlTags { get; set; }
        public Cache Cache { get; set; }
        public ICacheProvider CacheProvider { get; internal set; }
        const string SMART_DB_PREFIX = "$";

        private Regex replaceDbPrefixReg;
        private Regex GetReplaceDbPrefixReg()
        {
            String dbPrefix = SmartSqlMap.SmartSqlMapConfig.Database.DbProvider.ParameterPrefix;

            replaceDbPrefixReg = new Regex(@"([\p{L}\p{N}_]+)=([\"+ SMART_DB_PREFIX + @"])([\p{L}\p{N}_]+)", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled);

            return replaceDbPrefixReg;
        }




        public String BuildSql(RequestContext context)
        {
            context.SmartSqlMap = SmartSqlMap;
            string smartPrefix = SmartSqlMap.SmartSqlMapConfig.Settings.ParameterPrefix;
            String dbPrefix = SmartSqlMap.SmartSqlMapConfig.Database.DbProvider.ParameterPrefix;
            StringBuilder sqlStrBuilder = new StringBuilder();
            foreach (ITag tag in SqlTags)
            {
                sqlStrBuilder.Append(tag.BuildSql(context));
            }
            return sqlStrBuilder.Replace(smartPrefix, dbPrefix).ToString();
        }
    }
}
