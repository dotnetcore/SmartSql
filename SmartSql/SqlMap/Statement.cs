using SmartSql.Abstractions;
using SmartSql.Abstractions.Cache;
using SmartSql.Cache;
using SmartSql.Exceptions;
using SmartSql.SqlMap.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SmartSql.SqlMap
{
    public class Statement
    {
        [XmlIgnore]
        public SmartSqlMap SmartSqlMap { get; internal set; }
        [XmlAttribute]
        public String Id { get; set; }
        public String FullSqlId => $"{SmartSqlMap.Scope}.{Id}";
        public List<ITag> SqlTags { get; set; }
        public Cache Cache { get; set; }
        private ICacheProvider _cacheProvider;
        public ICacheProvider CacheProvider
        {
            get
            {
                #region Init CacheProvider
                if (_cacheProvider == null)
                {
                    lock (this)
                    {
                        if (_cacheProvider == null)
                        {
                            if (Cache == null)
                            {
                                _cacheProvider = new NoneCacheProvider();
                            }
                            else
                            {
                                _cacheProvider = Cache.CreateCacheProvider(this);
                            }
                        }
                    }
                }
                #endregion
                return _cacheProvider;
            }
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
