using SmartSql.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SmartSql.Configuration
{
    public class Properties
    {
        private IDictionary<string, string> _properties;
        private Regex _propertyTokens;
        public Properties()
        {
            var regOptions = RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled;
            _propertyTokens = new Regex(@"^\$\{([\p{L}\p{N}_]+)\}", regOptions);
        }

        public void Load(IDictionary<string, string> properties)
        {
            _properties = properties;
        }
        public void Load(IDictionary<string, object> properties)
        {
            _properties = new Dictionary<string, string>();
            foreach (var property in properties)
            {
                _properties.Add(property.Key, property.Value.ToString());
            }
        }
        public string GetPropertyValue(string propExp)
        {
            if (!_propertyTokens.IsMatch(propExp)) { return propExp; }
            return _propertyTokens.Replace(propExp, match =>
            {
                string propName = match.Groups[1].Value;
                if (!_properties.TryGetValue(propName, out var propVal))
                {
                    throw new SmartSqlException($"can not find Property.Name:{propName}.");
                }
                return propVal;
            });
        }
    }
}
