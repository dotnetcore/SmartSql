using SmartSql.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace SmartSql.SqlMap.Tags
{
    public abstract class Tag : ITag
    {
        [XmlAttribute]
        public virtual String Prepend { get; set; }
        [XmlAttribute]
        public String Property { get; set; }
        [XmlIgnore]
        public abstract TagType Type { get; }
        public IList<ITag> ChildTags { get; set; }
        [Obsolete("Removed In Tag")]
        public bool In { get; set; }
        public abstract bool IsCondition(RequestContext context);
        public virtual String BuildSql(RequestContext context)
        {
            if (IsCondition(context))
            {
                string dbPrefix = GetDbProviderPrefix(context);
                if (In)
                {
                    return $" {Prepend} In {dbPrefix}{Property} ";
                }

                StringBuilder strBuilder = BuildChildSql(context);
                return $" {Prepend}{strBuilder.ToString()}";
            }
            return String.Empty;
        }

        public virtual StringBuilder BuildChildSql(RequestContext context)
        {
            StringBuilder strBuilder = new StringBuilder();
            if (ChildTags != null && ChildTags.Count > 0)
            {
                foreach (var childTag in ChildTags)
                {
                    string strSql = childTag.BuildSql(context);
                    if (String.IsNullOrWhiteSpace(strSql))
                    {
                        continue;
                    }
                    strBuilder.Append(strSql);
                }
            }

            return strBuilder;
        }
        protected virtual String GetDbProviderPrefix(RequestContext context)
        {
            return context.SmartSqlMap.SmartSqlMapConfig.Database.DbProvider.ParameterPrefix;
        }

        protected virtual Object GetPropertyValue(RequestContext context)
        {
            var reqParams = context.RequestParameters;
            if (reqParams == null) { return null; }
            if (reqParams.ContainsKey(Property))
            {
                return reqParams[Property];
            }
            return null;
        }
    }
}
