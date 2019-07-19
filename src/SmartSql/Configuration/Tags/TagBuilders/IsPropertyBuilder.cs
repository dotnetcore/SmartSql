using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SmartSql.Configuration.Tags.TagBuilders
{
    public class IsPropertyBuilder : AbstractTagBuilder
    {
        public override ITag Build(XmlNode xmlNode, Statement statement)
        {
            return new IsProperty
            {
                Property = GetProperty(xmlNode),
                Prepend = GetPrepend(xmlNode),
                ChildTags = new List<ITag>(),
                PropertyChanged = PropertyChangedUtil.GetPropertyChanged(xmlNode, statement),
                Statement = statement
            };
        }
    }
}