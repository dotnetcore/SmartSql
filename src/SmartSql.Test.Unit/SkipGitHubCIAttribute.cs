using System;
using Xunit;

namespace SmartSql.Test.Unit;

public class SkipGitHubCIAttribute : FactAttribute
{
    public SkipGitHubCIAttribute()
    {
        if (IsGitHubCI())
        {
            Skip = "Ignore on GitHub CI.";
        }
    }

    private static bool IsGitHubCI()
        => Environment.GetEnvironmentVariable("GITHUB_ACTION") != null;
}