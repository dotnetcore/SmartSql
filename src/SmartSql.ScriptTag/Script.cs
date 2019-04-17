using System;
using SmartSql.Configuration.Tags;
using Jint;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace SmartSql.ScriptTag
{
    public class Script : Tag
    {
        private readonly Dictionary<string, string> _operatorMappings = new Dictionary<string, string>
            {
                { "and","&&" },
                { "or","||" },
                { "gt",">" },
                { "gte",">=" },
                { "lt","<" },
                { "lte","<=" },
                { "eq","==" },
                { "neq","!=" },
            };
        private readonly Regex _operatorRegex;
        public Script(string test)
        {
            _operatorRegex = new Regex($"\\s+({string.Join("|", _operatorMappings.Keys)})\\s+");
            Test = ParseOperator(test);
        }
        private string ParseOperator(string test)
        {
            return _operatorRegex.Replace(test, match =>
            {
                var opName = match.Groups[1].Value;
                return !_operatorMappings.TryGetValue(opName, out var op) ? match.Value : op;
            });
        }
        public String Test { get; }
        public override bool IsCondition(AbstractRequestContext context)
        {
            var engine = new Engine(options =>
            {
                options.Strict(false)
                    .DebugMode(false)
                    .AllowDebuggerStatement(false);
            });
            if (context.Parameters == null) return engine.Execute(Test).GetCompletionValue().AsBoolean();
            foreach (var reqParam in context.Parameters)
            {
                engine.SetValue(reqParam.Key, reqParam.Value.Value);
            }
            return engine.Execute(Test).GetCompletionValue().AsBoolean();
        }
    }
}
