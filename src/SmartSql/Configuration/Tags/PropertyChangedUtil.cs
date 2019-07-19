using System;
using System.Xml;
using SmartSql.Exceptions;

namespace SmartSql.Configuration.Tags
{
    public class PropertyChangedUtil
    {
        private const String PROPERTY_CHANGED = nameof(IPropertyChanged.PropertyChanged);

        /// <summary>
        /// 验证属性值变更状态是否满足要求
        /// </summary>
        /// <param name="propertyChanged"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool IsCondition(IPropertyChanged propertyChanged, AbstractRequestContext context)
        {
            if (!propertyChanged.PropertyChanged.HasValue)
            {
                return true;
            }

            var changedVersion = context.GetPropertyVersion(propertyChanged.Property);

            switch (changedVersion)
            {
                case -1:
                {
                    return true;
                }

                case 0:
                {
                    return !propertyChanged.PropertyChanged.Value;
                }

                case int version when version > 0:
                {
                    return propertyChanged.PropertyChanged.Value;
                }

                default:
                {
                    return false;
                }
            }
        }

        public static bool? GetPropertyChanged(XmlNode xmlNode, Statement statement)
        {
            string strVal = xmlNode.Attributes?[PROPERTY_CHANGED]?.Value?.Trim();
            if (String.IsNullOrEmpty(strVal))
            {
                if (statement.EnablePropertyChangedTrack)
                {
                    return statement.EnablePropertyChangedTrack;
                }

                return null;
            }

            if (!Boolean.TryParse(strVal, out var boolVal))
            {
                throw new SmartSqlException($"can not convert {strVal} to Boolean from xml-node:{xmlNode.Value}.");
            }

            return boolVal;

        }
    }
}