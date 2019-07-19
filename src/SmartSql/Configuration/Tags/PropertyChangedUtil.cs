namespace SmartSql.Configuration.Tags
{
    public class PropertyChangedUtil
    {
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
    }
}