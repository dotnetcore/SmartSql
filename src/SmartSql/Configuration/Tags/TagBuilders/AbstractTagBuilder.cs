using SmartSql.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SmartSql.Configuration.Tags.TagBuilders
{
    public abstract class AbstractTagBuilder : ITagBuilder
    {
        private const String PREPEND = "Prepend";
        private const String PROPERTY = "Property";
        private const String COMPARE_VALUE = "CompareValue";
        public abstract ITag Build(XmlNode xmlNode, Statement statement);

        public String GetXmlAttributeValue(XmlNode xmlNode, string attributeName)
        {
            return xmlNode.Attributes?[attributeName]?.Value?.Trim();
        }
        public Decimal GetXmlAttributeValueAsDecimal(XmlNode xmlNode, string attributeName)
        {
            string strVal = GetXmlAttributeValue(xmlNode, attributeName);
            if (!Decimal.TryParse(strVal, out decimal decimalVal))
            {
                throw new SmartSqlException();
            }
            return decimalVal;
        }

        public String GetPrepend(XmlNode xmlNode)
        {
            return GetXmlAttributeValue(xmlNode, PREPEND);
        }
        public String GetProperty(XmlNode xmlNode)
        {
            return GetXmlAttributeValue(xmlNode, PROPERTY);
        }
        public String GetCompareValue(XmlNode xmlNode)
        {
            return GetXmlAttributeValue(xmlNode, COMPARE_VALUE);
        }
        public Decimal GetCompareValueAsDecimal(XmlNode xmlNode)
        {
            return GetXmlAttributeValueAsDecimal(xmlNode, COMPARE_VALUE);
        }
    }
}
