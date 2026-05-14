using System;
using Xunit;

namespace SmartSql.Test.Integration;

public class EnvironmentFactAttribute : FactAttribute
{
    public const string GITHUB_ACTION = "GITHUB_ACTION";

    public EnvironmentFactAttribute(string include = null, string exclude = null)
    {
        Include = include;
        Exclude = exclude;

        if (!string.IsNullOrEmpty(include) && !ContainsEnvVar(include))
        {
            Skip = $"Skip: requires env [{include}].";
        }

        if (!string.IsNullOrEmpty(exclude) && ContainsEnvVar(exclude))
        {
            Skip = $"Skip: excluded by env [{exclude}].";
        }
    }

    public string Include { get; }
    public string Exclude { get; }

    private static bool ContainsEnvVar(string envVar)
        => Environment.GetEnvironmentVariable(envVar) != null;
}
