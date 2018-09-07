using SmartSql.Utils.PropertyAccessor;
using SmartSql.Utils.PropertyAccessor.Impl;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Utils
{
    public static class GetAccessorUtil
    {
        public static PropertyValue GetValue(object target, string propertyName, bool ignorePropertyCase)
        {
            if (target == null) { return PropertyValue.NullTarget; }
            var getAccessorFactory = GetAccessorFactory.Instance;
            if (propertyName.IndexOf('.') > -1)
            {
                var pNames = propertyName.Split('.');
                PropertyValue propertyVal = new PropertyValue { Value = target };
                for (int i = 0; i < pNames.Length; i++)
                {
                    if (propertyVal.Value == null) { return PropertyValue.NullTarget; }
                    var childName = pNames[i];
                    var getChildParamVal = getAccessorFactory.CreateGet(propertyVal.Value.GetType(), childName, false);
                    propertyVal = getChildParamVal(propertyVal.Value);
                }
                return propertyVal;
            }
            else
            {
                var getVal = getAccessorFactory.CreateGet(target.GetType(), propertyName, ignorePropertyCase);
                return getVal(target);
            }
        }
    }
}
