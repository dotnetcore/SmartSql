using SmartSql.Utils.PropertyAccessor.Impl;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Utils
{
    public static class GetAccessorUtil
    {
        public static object GetValue(object target, string propertyName, bool ignorePropertyCase)
        {
            if (target == null) { return null; }
            var getAccessorFactory = GetAccessorFactory.Instance;
            if (propertyName.IndexOf('.') > -1)
            {
                var pNames = propertyName.Split('.');
                for (int i = 0; i < pNames.Length; i++)
                {
                    if (target == null) { return null; }
                    var childName = pNames[i];
                    var getChildParamVal = getAccessorFactory.CreateGet(target.GetType(), childName, false);
                    target = getChildParamVal(target);
                }
                return target;
            }
            else
            {
                var getVal = getAccessorFactory.CreateGet(target.GetType(), propertyName, ignorePropertyCase);
                return getVal(target);
            }
        }
    }
}
