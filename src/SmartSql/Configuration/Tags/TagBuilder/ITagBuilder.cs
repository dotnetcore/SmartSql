using SmartSql.Configuration.Statements;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SmartSql.Configuration.Tags.TagBuilder
{
    public interface ITagBuilder
    {
        bool Filter(string nodeName);
        ITag BuildTag(XmlNode xmlNode, Statement statement);
    }
}
