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
            if (propertyChanged.PropertyChanged == PropertyChangedState.Ignore)
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
                    return propertyChanged.PropertyChanged == PropertyChangedState.Unchanged;
                }

                case int version when version > 0:
                {
                    return propertyChanged.PropertyChanged == PropertyChangedState.Changed;
                }

                default:
                {
                    return false;
                }
            }
        }

        public static PropertyChangedState GetPropertyChanged(XmlNode xmlNode, Statement statement)
        {
            string strVal = xmlNode.Attributes?[PROPERTY_CHANGED]?.Value?.Trim();
            if (String.IsNullOrEmpty(strVal))
            {
                if (statement.EnablePropertyChangedTrack)
                {
                    return PropertyChangedState.Changed;
                }

                return PropertyChangedState.Ignore;
            }

            switch (strVal)
            {
                case nameof(PropertyChangedState.Ignore):
                {
                    return PropertyChangedState.Ignore;
                }

                case nameof(PropertyChangedState.Changed):
                {
                    return PropertyChangedState.Changed;
                }

                case nameof(PropertyChangedState.Unchanged):
                {
                    return PropertyChangedState.Unchanged;
                }

                default:
                {
                    throw new SmartSqlException(
                        $"can not convert {strVal} to PropertyChangedState from xml-node:{xmlNode.Value}.");
                }
            }
        }
    }
}