using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SmartSql.DyRepository
{
    public class ScopeTemplateParser
    {
        private const string DEFAULT_SCOPE_TEMPLATE = "I{Scope}Repository";
        private readonly Regex _repositoryScope;
        public ScopeTemplateParser(string template = "")
        {
            if (String.IsNullOrEmpty(template))
            {
                template = DEFAULT_SCOPE_TEMPLATE;
            }
            template = template.Replace("{Scope}", @"([\p{L}\p{N}_]+)");
            template = template.Insert(0, "^");
            template = template + "$";
            _repositoryScope = new Regex(template, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        }
        public String Parse(string repositoryName)
        {
            var matchScope = _repositoryScope.Match(repositoryName);
            return matchScope.Groups[1].Value;
        }
    }
}
