using System;
using Xunit;

namespace SmartSql.Test.Unit
{
    public class SkipGitHubActionAttribute : FactAttribute
    {
        public SkipGitHubActionAttribute()
        {
            if (IsGitHubAction())
            {
                Skip = "Ignore on GitHub CI.";
            }
        }

        public static bool IsGitHubAction()
            => Environment.GetEnvironmentVariable("GITHUB_ACTION") != null;
    }
}