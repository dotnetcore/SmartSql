using System;
using Xunit;

namespace SmartSql.Test.Unit
{
    public class EnvironmentFactAttribute : FactAttribute
    {
        public const String GITHUB_ACTION = "GITHUB_ACTION";

        public EnvironmentFactAttribute(String include = null, String exclude = null)
        {
            Include = include;
            Exclude = exclude;

            if (!String.IsNullOrEmpty(include) && !ContainsEnvVar(include))
            {
                Skip = $"Ignore on Environment When Not Include:[{include}].";
            }

            if (!String.IsNullOrEmpty(exclude) && ContainsEnvVar(exclude))
            {
                Skip = $"Ignore on Environment When Contains Include:[{include}].";
            }

        }
        
        public String Include { get; }
        public String Exclude { get; }

        private static bool ContainsEnvVar(String envVar)
            => Environment.GetEnvironmentVariable(envVar) != null;
    }
}