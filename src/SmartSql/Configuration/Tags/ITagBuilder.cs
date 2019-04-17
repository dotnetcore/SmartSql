using System.Xml;

namespace SmartSql.Configuration.Tags
{
    public interface ITagBuilder
    {
        ITag Build(XmlNode xmlNode, Statement statement);
    }
}
