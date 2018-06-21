using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SmartSql.Configuration.Tags.TagBuilder
{
    public interface ITagBuilder
    {
        IEnumerable<String> NodeNames { get; }
        bool Filter(string nodeName);
        ITag Build(XmlNode xmlNode);
    }
}
