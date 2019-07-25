using SmartSql.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SmartSql.Configuration.Tags.TagBuilders
{
    public abstract class AbstractTagBuilder : ITagBuilder
    {
        private const String PREPEND = nameof(Tag.Prepend);
        private const String PROPERTY = nameof(Tag.Property);
        private const String REQUIRED = nameof(Tag.Required);
        private const String MIN = nameof(Dynamic.Min);
        private const String COMPARE_VALUE = nameof(NumericalCompareTag.CompareValue);

        public abstract ITag Build(XmlNode xmlNode, Statement statement);

        public String GetXmlAttributeValue(XmlNode xmlNode, string attributeName)
        {
            return xmlNode.Attributes?[attributeName]?.Value?.Trim();
        }

        public Decimal GetXmlAttributeValueAsDecimal(XmlNode xmlNode, string attributeName)
        {
            string strVal = GetXmlAttributeValue(xmlNode, attributeName);
            if (!Decimal.TryParse(strVal, out var decimalVal))
            {
                throw new SmartSqlException($"can not convert {strVal} to decimal from xml-node -> {nameof(xmlNode.BaseURI)}:[{xmlNode.BaseURI}],[{nameof(xmlNode.OuterXml)}]:[{xmlNode.OuterXml}].");
            }

            return decimalVal;
        }

        public String GetPrepend(XmlNode xmlNode)
        {
            return GetXmlAttributeValue(xmlNode, PREPEND);
        }

        public String GetProperty(XmlNode xmlNode)
        {
            if (xmlNode.Attributes.TryGetValueAsString(PROPERTY, out var propertyStr))
            {
                return propertyStr;
            }
            throw new SmartSqlException($"can not find [{PROPERTY}] from xml-node -> {nameof(xmlNode.BaseURI)}:[{xmlNode.BaseURI}],[{nameof(xmlNode.OuterXml)}]:[{xmlNode.OuterXml}].");
        }

        public bool GetRequired(XmlNode xmlNode)
        {
            xmlNode.Attributes.TryGetValueAsBoolean(REQUIRED, out var requiredVal);
            return requiredVal;
        }

        public Int32? GetMin(XmlNode xmlNode)
        {
            if (xmlNode.Attributes.TryGetValueAsInt32(MIN, out var requiredVal))
            {
                return requiredVal;
            }

            return null;
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