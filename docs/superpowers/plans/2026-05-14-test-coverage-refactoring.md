# Test Coverage & Convention Refactoring Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Refactor all 76 existing unit tests to follow consistent naming (`Should_X_When_Y`), FluentAssertions, AAA structure, then add new tests to raise coverage from 19.4%.

**Architecture:** Process each module independently: refactor existing tests first, then add new tests. Each module = one task = one commit.

**Tech Stack:** xUnit 2.9.3, FluentAssertions 7.1.0, Moq 4.20.72, .NET 8.0

---

## File Structure

### Modified files (refactored tests)
- `src/SmartSql.Test.Unit/Cryptos/AESCyptoTest.cs` → rename to `AESCryptoTests.cs`
- `src/SmartSql.Test.Unit/Cryptos/DESCryptoTest.cs` → rename to `DESCryptoTests.cs`
- `src/SmartSql.Test.Unit/Cryptos/RSACryptoTest.cs` → rename to `RSACryptoTests.cs`
- `src/SmartSql.Test.Unit/ConfigBuilder/PropertiesTest.cs` → rename to `PropertiesTests.cs`
- `src/SmartSql.Test.Unit/ConfigBuilder/XmlConfigLoaderTest.cs` → rename to `XmlConfigLoaderTests.cs`
- `src/SmartSql.Test.Unit/Utils/*.cs` — all 6 files renamed and refactored
- `src/SmartSql.Test.Unit/PipelineBuilderTest.cs` → rename to `PipelineBuilderTests.cs`
- `src/SmartSql.Test.Unit/Reflection/*.cs` — all 10 files renamed and refactored
- `src/SmartSql.Test.Unit/Tags/ScriptTest.cs` → rename to `ScriptTests.cs`
- `src/SmartSql.Test.Unit/Tags/SqlTextTest.cs` → rename to `SqlTextTests.cs`
- `src/SmartSql.Test.Unit/TypeHandlers/*.cs` — all 4 files renamed and refactored

### New files (additional tests)
- `src/SmartSql.Test.Unit/Tags/IsNotEmptyTests.cs`
- `src/SmartSql.Test.Unit/Tags/IsEqualTests.cs`
- `src/SmartSql.Test.Unit/Tags/IsGreaterThanTests.cs`
- `src/SmartSql.Test.Unit/Tags/IsLessThanTests.cs`
- `src/SmartSql.Test.Unit/Tags/WhereTests.cs`
- `src/SmartSql.Test.Unit/Tags/SetTests.cs`
- `src/SmartSql.Test.Unit/Tags/ForTests.cs`
- `src/SmartSql.Test.Unit/Tags/DynamicTests.cs`
- `src/SmartSql.Test.Unit/Tags/IncludeTests.cs`
- `src/SmartSql.Test.Unit/Tags/EnvTests.cs`
- `src/SmartSql.Test.Unit/Tags/SwitchTests.cs`
- `src/SmartSql.Test.Unit/Cache/FifoCacheProviderTests.cs`
- `src/SmartSql.Test.Unit/Cache/LruCacheProviderTests.cs`
- `src/SmartSql.Test.Unit/Cache/CacheKeyTests.cs`
- `src/SmartSql.Test.Unit/TypeHandlers/TypeHandlerFactoryTests.cs` (new tests added to existing)
- `src/SmartSql.Test.Unit/Configuration/PropertiesTests.cs` (additional tests)

---

## Task 1: Refactor Cryptos tests (3 files)

**Files:**
- Modify: `src/SmartSql.Test.Unit/Cryptos/AESCyptoTest.cs`
- Modify: `src/SmartSql.Test.Unit/Cryptos/DESCryptoTest.cs`
- Modify: `src/SmartSql.Test.Unit/Cryptos/RSACryptoTest.cs`

- [ ] **Step 1: Refactor AESCryptoTests**

Replace `src/SmartSql.Test.Unit/Cryptos/AESCyptoTest.cs` entirely with:

```csharp
using System.Collections.Generic;
using FluentAssertions;
using SmartSql.TypeHandler.Crypto;
using Xunit;

namespace SmartSql.Test.Unit.Cryptos;

public class AESCryptoTests
{
    private readonly Dictionary<string, object> _defaultConfig = new Dictionary<string, object>
    {
        {"Key", "awVFRYPeTTrA9T7OOzaAFUvu8I/ZyYjAtIzEjCmzzYw="},
        {"IV", "7cFxoI3/k1wxN9P6rEyR/Q=="}
    };

    [Fact]
    public void Should_RoundTrip_When_EncryptAndDecrypt()
    {
        using var crypto = new AESCrypto();
        crypto.Initialize(_defaultConfig);
        var plainText = "SmartSql";

        var cipherText = crypto.Encrypt(plainText);
        var decryptText = crypto.Decrypt(cipherText);

        decryptText.Should().Be(plainText);
    }

    [Fact]
    public void Should_RoundTrip_When_EncryptAndDecryptMultipleTimes()
    {
        using var crypto = new AESCrypto();
        crypto.Initialize(_defaultConfig);
        var plainText = "SmartSql";

        for (int i = 0; i < 3; i++)
        {
            var cipherText = crypto.Encrypt(plainText);
            var decryptText = crypto.Decrypt(cipherText);
            decryptText.Should().Be(plainText);
        }
    }

    [Fact]
    public void Should_RoundTrip_When_LargePlainText()
    {
        using var crypto = new AESCrypto();
        crypto.Initialize(_defaultConfig);
        var plainText = new string('A', 10000);

        var cipherText = crypto.Encrypt(plainText);
        var decryptText = crypto.Decrypt(cipherText);

        decryptText.Should().Be(plainText);
    }
}
```

- [ ] **Step 2: Refactor DESCryptoTests**

Replace `src/SmartSql.Test.Unit/Cryptos/DESCryptoTest.cs` entirely with:

```csharp
using System.Collections.Generic;
using FluentAssertions;
using SmartSql.TypeHandler.Crypto;
using Xunit;

namespace SmartSql.Test.Unit.Cryptos;

public class DESCryptoTests
{
    private readonly Dictionary<string, object> _defaultConfig = new Dictionary<string, object>
    {
        {"Key", "qxMfZpmQ1Rk="},
        {"IV", "XaX73vwx694="}
    };

    [Fact]
    public void Should_RoundTrip_When_EncryptAndDecrypt()
    {
        using var crypto = new DESCrypto();
        crypto.Initialize(_defaultConfig);
        var plainText = "SmartSql";

        var cipherText = crypto.Encrypt(plainText);
        var decryptText = crypto.Decrypt(cipherText);

        decryptText.Should().Be(plainText);
    }
}
```

- [ ] **Step 3: Refactor RSACryptoTests**

Replace `src/SmartSql.Test.Unit/Cryptos/RSACryptoTest.cs` entirely with:

```csharp
using System.Collections.Generic;
using FluentAssertions;
using SmartSql.TypeHandler.Crypto;
using Xunit;

namespace SmartSql.Test.Unit.Cryptos;

public class RSACryptoTests
{
    private readonly Dictionary<string, object> _defaultConfig = new Dictionary<string, object>
    {
        {"PublicKey", "<RSAKeyValue><Modulus>oBEYkMB1Ol2G+1M7n0e5k+LtzYnXTvGeVVysmy5d5mHqvUUqG6T4jAJbjjbR+x6NKokNqMjT2Y0s0YXHcJLiJfT1EFyzj24bOlE8MMiN9oVYi1m+3tzM+JYL6AdLul6qW+HJn4T2yGq2DK8phvuTStBCL7P9bN3p3rHzFODP6eE=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>"},
        {"PrivateKey", "<RSAKeyValue><Modulus>oBEYkMB1Ol2G+1M7n0e5k+LtzYnXTvGeVVysmy5d5mHqvUUqG6T4jAJbjjbR+x6NKokNqMjT2Y0s0YXHcJLiJfT1EFyzj24bOlE8MMiN9oVYi1m+3tzM+JYL6AdLul6qW+HJn4T2yGq2DK8phvuTStBCL7P9bN3p3rHzFODP6eE=</Modulus><Exponent>AQAB</Exponent><D>T9qX0UnBBiLbx2cKre+BNzBdENW9UZ4TQfa+F5mFVODZk/xVN3fJU1h6M5c9lX8umwGBjqNvh6H2CmIYwG9Y6ZktEayRvmBq0HhFRjT2q/vZ0qXFq7CKKv8Hj66lnt3pIhLx9Q0mrkgcL9A1s+UtFAz1FxXtnqfi3KxV2DCE=</D><P>5uBmzKqHCqTxuE2CJESkqGCi+LuM/KH9W L3n+THJq2JUbsJ7cYfbE0vq8Kk0JBZ0sI4mWbN8c3YohZ2cB1nQw==</P><Q>wwPk0n9c9PFMi1H55Kx1pPTdCHFCVCSap9q3FrcJ1QIK0P8YQdZNKNwqo0ruN2FVSa6byGxbKGc6zaZJQXJ1Tw==</Q><DP>L5HY1WYHJOYBuKXbT6p1fY6c3cDaLBlXvd1Z9xexAoMA9q6aS3Bz8a3pW76o6EMkyGMSI0MFzB9KMBJP3JeiIQ==</DP><DQ>hbW06pU/Tp0BWjM6AjnZHNCW50vb2nG6D4cvx2bqBhVGyWKMHUH9W6mnf/OBQwOx1rwlX7sMAiLZKC3T00jWwQ==</DQ><InverseQ>WE1QKEtWV1NrVEl0TFY5R2prSWRaMHcKQmZiTzlGQVpBNjZFaTVPZktBclVFd2R2Z0NNSDZubm5ZaW8K</InverseQ></RSAKeyValue>"}
    };

    [Fact]
    public void Should_RoundTrip_When_EncryptAndDecrypt()
    {
        using var crypto = new RSACrypto();
        crypto.Initialize(_defaultConfig);
        var plainText = "SmartSql";

        var cipherText = crypto.Encrypt(plainText);
        var decryptText = crypto.Decrypt(cipherText);

        decryptText.Should().Be(plainText);
    }
}
```

Note: If the RSA test requires valid keys, check the original `RSACryptoTest.cs` in the Integration project for the correct key values. The RSA test was previously commented out (`// [Fact]`). If the crypto implementation requires specific key formats that don't work with the above, skip this test or use valid test keys from the original source.

- [ ] **Step 4: Run tests**

Run: `dotnet test src/SmartSql.Test.Unit/SmartSql.Test.Unit.csproj --filter "FullyQualifiedName~Cryptos"`
Expected: All tests pass

- [ ] **Step 5: Commit**

```bash
git add -A
git commit -m "refactor: refactor Cryptos tests with naming convention and FluentAssertions"
```

---

## Task 2: Refactor ConfigBuilder tests (2 files)

**Files:**
- Modify: `src/SmartSql.Test.Unit/ConfigBuilder/PropertiesTest.cs`
- Modify: `src/SmartSql.Test.Unit/ConfigBuilder/XmlConfigLoaderTest.cs`

- [ ] **Step 1: Refactor PropertiesTests**

Replace `src/SmartSql.Test.Unit/ConfigBuilder/PropertiesTest.cs` entirely with:

```csharp
using System.Collections.Generic;
using FluentAssertions;
using SmartSql.Configuration;
using Xunit;

namespace SmartSql.Test.Unit.ConfigBuilder;

public class PropertiesTests
{
    [Fact]
    public void Should_ResolveValue_When_KeyExists()
    {
        var properties = new Properties();
        properties.Import(new Dictionary<string, string> {{"SmartSql", "Great"}});

        var result = properties.GetPropertyValue("${SmartSql}");

        result.Should().Be("Great");
    }

    [Fact]
    public void Should_ResolveValue_When_AppendedToText()
    {
        var properties = new Properties();
        properties.Import(new Dictionary<string, string> {{"SmartSql", "Great"}});

        var result = properties.GetPropertyValue("${SmartSql}-Great");

        result.Should().Be("Great-Great");
    }

    [Fact]
    public void Should_ResolveValue_When_KeyContainsColon()
    {
        var properties = new Properties();
        properties.Import(new Dictionary<string, string> {{"SmartSql:Great", "Yes"}});

        var result = properties.GetPropertyValue("${SmartSql:Great}");

        result.Should().Be("Yes");
    }

    [Fact]
    public void Should_ResolveValue_When_KeyContainsBackQuote()
    {
        var properties = new Properties();
        properties.Import(new Dictionary<string, string> {{"SmartSql`Great", "Yes"}});

        var result = properties.GetPropertyValue("${SmartSql`Great}");

        result.Should().Be("Yes");
    }

    [Fact]
    public void Should_ResolveValue_When_KeyContainsNumber()
    {
        var properties = new Properties();
        properties.Import(new Dictionary<string, string> {{"SmartSql888", "Yes"}});

        var result = properties.GetPropertyValue("${SmartSql888}");

        result.Should().Be("Yes");
    }

    [Fact]
    public void Should_ResolveValue_When_KeyContainsDot()
    {
        var properties = new Properties();
        properties.Import(new Dictionary<string, string> {{"SmartSql.888", "Yes"}});

        var result = properties.GetPropertyValue("${SmartSql.888}");

        result.Should().Be("Yes");
    }

    [Fact]
    public void Should_ResolveValue_When_KeyContainsSpace()
    {
        var properties = new Properties();
        properties.Import(new Dictionary<string, string> {{"SmartSql 888", "Yes"}});

        var result = properties.GetPropertyValue("${SmartSql 888}");

        result.Should().Be("Yes");
    }

    [Fact]
    public void Should_ResolveValue_When_ConcatWithPrefix()
    {
        var properties = new Properties();
        properties.Import(new Dictionary<string, string> {{"SmartSql", "Great"}});

        var result = properties.GetPropertyValue("SmartSql.${SmartSql}");

        result.Should().Be("SmartSql.Great");
    }

    [Fact]
    public void Should_ReturnRawString_When_KeyNotFound()
    {
        var properties = new Properties();

        var result = properties.GetPropertyValue("${Unknown}");

        result.Should().Be("${Unknown}");
    }
}
```

- [ ] **Step 2: Refactor XmlConfigLoaderTests**

Replace `src/SmartSql.Test.Unit/ConfigBuilder/XmlConfigLoaderTest.cs` entirely with:

```csharp
using FluentAssertions;
using SmartSql.ConfigBuilder;
using Xunit;

namespace SmartSql.Test.Unit.ConfigBuilder;

public class XmlConfigLoaderTests
{
    [Fact]
    public void Should_LoadConfig_When_ValidXmlFile()
    {
        var configLoader = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-UnitTest.xml");

        var config = configLoader.Build();

        config.Should().NotBeNull();
    }
}
```

Note: `SmartSqlMapConfig-UnitTest.xml` is the minimal SQLite config created during migration. If it uses a different name, update accordingly.

- [ ] **Step 3: Run tests**

Run: `dotnet test src/SmartSql.Test.Unit/SmartSql.Test.Unit.csproj --filter "FullyQualifiedName~ConfigBuilder"`
Expected: All tests pass

- [ ] **Step 4: Commit**

```bash
git add -A
git commit -m "refactor: refactor ConfigBuilder tests with naming convention and FluentAssertions"
```

---

## Task 3: Refactor Utils tests (6 files)

**Files:**
- Modify: all files in `src/SmartSql.Test.Unit/Utils/`

For each file, rename the class to plural (e.g., `WeightFilterTest` → `WeightFilterTests`), rename methods to `Should_X_When_Y` pattern, replace `Assert.Equal` with `Should().Be()`, replace `Assert.NotNull` with `Should().NotBeNull()`, add `using FluentAssertions;`, separate AAA sections with blank lines.

The refactoring is mechanical for all 6 files. Key method renames:

- `WeightFilterTest.Elect()` → `WeightFilterTests.Should_ElectDataSource_When_WeightSourcesProvided()`
- `SqlParamAnalyzerTest.Analyse_NonParam()` → `SqlParamAnalyzerTests.Should_ReturnOriginalSql_When_NoParameters()`
- `SqlParamAnalyzerTest.Analyse()` → `SqlParamAnalyzerTests.Should_ReplaceParameters_When_SqlHasParameters()`
- `TableNameAnalyzerTest` methods → `Should_ConvertTableName_When_InsertStatement()` etc.
- `InsertWithIdTest.Replace()` → `Should_ReplaceIdPlaceholder_When_SqlContainsId()`
- `InsertWithIdTest.Replace2()` → `Should_ReplaceIdPlaceholder_When_SqlHasColumnList()`
- `ResourceUtilTest.LoadUriAsXml()` → `Should_LoadXml_When_UriProvided()` etc.
- `ValueTupleConvertTest` methods → follow naming pattern

- [ ] **Step 1: Refactor all 6 Utils test files**

Apply the refactoring pattern to each file. Read each file first to understand its structure, then rewrite with new naming, FluentAssertions, and AAA structure.

- [ ] **Step 2: Run tests**

Run: `dotnet test src/SmartSql.Test.Unit/SmartSql.Test.Unit.csproj --filter "FullyQualifiedName~Utils"`
Expected: All tests pass

- [ ] **Step 3: Commit**

```bash
git add -A
git commit -m "refactor: refactor Utils tests with naming convention and FluentAssertions"
```

---

## Task 4: Refactor PipelineBuilder, Reflection, Tags, TypeHandlers tests (17 files)

**Files:**
- Modify: `src/SmartSql.Test.Unit/PipelineBuilderTest.cs`
- Modify: all 10 files in `src/SmartSql.Test.Unit/Reflection/`
- Modify: `src/SmartSql.Test.Unit/Tags/ScriptTest.cs`
- Modify: `src/SmartSql.Test.Unit/Tags/SqlTextTest.cs`
- Modify: all 4 files in `src/SmartSql.Test.Unit/TypeHandlers/`

Same mechanical refactoring as Tasks 1-3. Key renames:

**PipelineBuilderTest** → `PipelineBuilderTests`:
- `Build()` → `Should_BuildOrderedPipeline_When_AllMiddlewareAdded()`

**ScriptTest** → `ScriptTests`:
- `And()` → `Should_ReturnTrue_When_ScriptAndConditionMet()`
- `Or()` → `Should_ReturnTrue_When_ScriptOrConditionMet()`
- `ArrayIndex()` → `Should_ReturnTrue_When_ScriptArrayIndexConditionMet()`
- `Eq()` → `Should_ReturnTrue_When_ScriptEqualityHolds()`
- `GreatThen()` → `Should_ReturnTrue_When_ScriptGreaterThanConditionMet()`
- `LessThen()` → `Should_ReturnTrue_When_ScriptLessThanConditionMet()`

**SqlTextTest** → `SqlTextTests`:
- `BuildSql()` → `Should_BuildSql_When_NoInSyntax()`
- `BuildSqlWithIn()` → `Should_ExpandInClause_When_ParameterIsArray()`
- `BuildSqlWithInAndSemicolon()` → `Should_ExpandInClause_When_FollowedBySemicolon()`

**TypeHandlers** — rename classes to plural, methods to `Should_X_When_Y`.

**Reflection** — rename classes to plural (e.g., `PropertyTokenizerTest` → `PropertyTokenizerTests`), methods to `Should_X_When_Y`.

- [ ] **Step 1: Refactor all 17 files**

Apply the refactoring pattern to each file.

- [ ] **Step 2: Run all unit tests**

Run: `dotnet test src/SmartSql.Test.Unit/SmartSql.Test.Unit.csproj`
Expected: All tests pass

- [ ] **Step 3: Commit**

```bash
git add -A
git commit -m "refactor: refactor PipelineBuilder, Reflection, Tags, TypeHandlers tests"
```

---

## Task 5: Add Tags tests — IsNotEmpty, IsEqual, IsGreaterThan, IsLessThan

**Files:**
- Create: `src/SmartSql.Test.Unit/Tags/IsNotEmptyTests.cs`
- Create: `src/SmartSql.Test.Unit/Tags/IsEqualTests.cs`
- Create: `src/SmartSql.Test.Unit/Tags/IsGreaterThanTests.cs`
- Create: `src/SmartSql.Test.Unit/Tags/IsLessThanTests.cs`

These tags test conditions based on request parameters. The testing pattern is:
1. Create the tag instance (set `Property`)
2. Create a `RequestContext` with parameters
3. Call `IsCondition(context)` and assert result
4. Optionally call `BuildSql(context)` and verify SQL output

**Important:** These tags inherit from `Tag` which has `EnsurePropertyValue` that reads from `context.Parameters`. The `RequestContext` needs `Parameters` populated. Use `SqlParameterCollection` to set up parameters.

- [ ] **Step 1: Create IsNotEmptyTests**

```csharp
using FluentAssertions;
using SmartSql.Configuration.Tags;
using SmartSql.Data;
using Xunit;

namespace SmartSql.Test.Unit.Tags;

public class IsNotEmptyTests
{
    private IsNotEmpty CreateTag(string property)
    {
        return new IsNotEmpty { Property = property };
    }

    private RequestContext CreateContext(params (string key, object value)[] parameters)
    {
        var sqlParams = new SqlParameterCollection();
        foreach (var (key, value) in parameters)
        {
            sqlParams.TryAdd(key, value);
        }
        var context = new RequestContext { Request = sqlParams };
        context.SetupParameters();
        return context;
    }

    [Fact]
    public void Should_ReturnFalse_When_PropertyIsNull()
    {
        var tag = CreateTag("Name");
        var context = CreateContext();

        tag.IsCondition(context).Should().BeFalse();
    }

    [Fact]
    public void Should_ReturnFalse_When_PropertyIsEmptyString()
    {
        var tag = CreateTag("Name");
        var context = CreateContext(("Name", ""));

        tag.IsCondition(context).Should().BeFalse();
    }

    [Fact]
    public void Should_ReturnTrue_When_PropertyIsNonEmptyString()
    {
        var tag = CreateTag("Name");
        var context = CreateContext(("Name", "SmartSql"));

        tag.IsCondition(context).Should().BeTrue();
    }

    [Fact]
    public void Should_ReturnFalse_When_PropertyIsEmptyCollection()
    {
        var tag = CreateTag("Ids");
        var context = CreateContext(("Ids", new int[0]));

        tag.IsCondition(context).Should().BeFalse();
    }

    [Fact]
    public void Should_ReturnTrue_When_PropertyIsNonEmptyCollection()
    {
        var tag = CreateTag("Ids");
        var context = CreateContext(("Ids", new[] { 1, 2, 3 }));

        tag.IsCondition(context).Should().BeTrue();
    }
}
```

Note: `IsNotEmpty` uses `EnsurePropertyValue` which reads from `context.Parameters`. The parameters come from the `Request` object. If `SetupParameters()` doesn't propagate correctly, try setting `context.Parameters` directly or using `SqlParameterCollection` as the `Request` object.

If `IsCondition` requires `context.ExecutionContext` (via `EnsurePropertyValue` → `context.Parameters.TryGetParameterValue`), you may need to mock the execution context. In that case, set parameters directly on the context:

```csharp
private RequestContext CreateContext(params (string key, object value)[] parameters)
{
    var sqlParams = new SqlParameterCollection();
    foreach (var (key, value) in parameters)
    {
        sqlParams.TryAdd(key, value);
    }
    return new RequestContext { Request = sqlParams };
    // SetupParameters will set Parameters from Request
}
```

Read the `Tag.EnsurePropertyValue` source to verify how it accesses parameters before writing tests. The method calls `context.Parameters.TryGetParameterValue(Property, out object paramVal)`.

- [ ] **Step 2: Create IsEqualTests**

```csharp
using FluentAssertions;
using SmartSql.Configuration.Tags;
using SmartSql.Data;
using Xunit;

namespace SmartSql.Test.Unit.Tags;

public class IsEqualTests
{
    private IsEqual CreateTag(string property, string compareValue)
    {
        return new IsEqual { Property = property, CompareValue = compareValue };
    }

    private RequestContext CreateContext(params (string key, object value)[] parameters)
    {
        var sqlParams = new SqlParameterCollection();
        foreach (var (key, value) in parameters)
        {
            sqlParams.TryAdd(key, value);
        }
        return new RequestContext { Request = sqlParams };
    }

    [Fact]
    public void Should_ReturnTrue_When_PropertyEqualsCompareValue()
    {
        var tag = CreateTag("Status", "Active");
        var context = CreateContext(("Status", "Active"));
        context.SetupParameters();

        tag.IsCondition(context).Should().BeTrue();
    }

    [Fact]
    public void Should_ReturnFalse_When_PropertyNotEqualsCompareValue()
    {
        var tag = CreateTag("Status", "Active");
        var context = CreateContext(("Status", "Inactive"));
        context.SetupParameters();

        tag.IsCondition(context).Should().BeFalse();
    }

    [Fact]
    public void Should_ReturnFalse_When_PropertyIsNull()
    {
        var tag = CreateTag("Status", "Active");
        var context = CreateContext();

        tag.IsCondition(context).Should().BeFalse();
    }
}
```

- [ ] **Step 3: Create IsGreaterThanTests**

```csharp
using FluentAssertions;
using SmartSql.Configuration.Tags;
using SmartSql.Data;
using Xunit;

namespace SmartSql.Test.Unit.Tags;

public class IsGreaterThanTests
{
    private IsGreaterThan CreateTag(string property, decimal compareValue)
    {
        return new IsGreaterThan { Property = property, CompareValue = compareValue };
    }

    private RequestContext CreateContext(params (string key, object value)[] parameters)
    {
        var sqlParams = new SqlParameterCollection();
        foreach (var (key, value) in parameters)
        {
            sqlParams.TryAdd(key, value);
        }
        return new RequestContext { Request = sqlParams };
    }

    [Fact]
    public void Should_ReturnTrue_When_PropertyGreaterThanCompareValue()
    {
        var tag = CreateTag("Age", 18M);
        var context = CreateContext(("Age", 25));
        context.SetupParameters();

        tag.IsCondition(context).Should().BeTrue();
    }

    [Fact]
    public void Should_ReturnFalse_When_PropertyEqualsCompareValue()
    {
        var tag = CreateTag("Age", 18M);
        var context = CreateContext(("Age", 18));
        context.SetupParameters();

        tag.IsCondition(context).Should().BeFalse();
    }

    [Fact]
    public void Should_ReturnFalse_When_PropertyLessThanCompareValue()
    {
        var tag = CreateTag("Age", 18M);
        var context = CreateContext(("Age", 10));
        context.SetupParameters();

        tag.IsCondition(context).Should().BeFalse();
    }

    [Fact]
    public void Should_ReturnFalse_When_PropertyIsNull()
    {
        var tag = CreateTag("Age", 18M);
        var context = CreateContext();

        tag.IsCondition(context).Should().BeFalse();
    }
}
```

- [ ] **Step 4: Create IsLessThanTests**

Follow the same pattern as `IsGreaterThanTests` but test `<` comparison:
- `Should_ReturnTrue_When_PropertyLessThanCompareValue`
- `Should_ReturnFalse_When_PropertyEqualsCompareValue`
- `Should_ReturnFalse_When_PropertyGreaterThanCompareValue`
- `Should_ReturnFalse_When_PropertyIsNull`

- [ ] **Step 5: Run tests**

Run: `dotnet test src/SmartSql.Test.Unit/SmartSql.Test.Unit.csproj --filter "FullyQualifiedName~Tags"`
Expected: All tests pass

- [ ] **Step 6: Commit**

```bash
git add -A
git commit -m "test: add IsNotEmpty, IsEqual, IsGreaterThan, IsLessThan tag tests"
```

---

## Task 6: Add Tags tests — Where, Set, Dynamic, For

**Files:**
- Create: `src/SmartSql.Test.Unit/Tags/WhereTests.cs`
- Create: `src/SmartSql.Test.Unit/Tags/SetTests.cs`
- Create: `src/SmartSql.Test.Unit/Tags/DynamicTests.cs`
- Create: `src/SmartSql.Test.Unit/Tags/ForTests.cs`

These tags have `ChildTags` and need more setup. The pattern:
1. Create the parent tag
2. Create child `SqlText` tags and add to `ChildTags`
3. Create context with parameters
4. Call `BuildSql` and verify the output

**Where and Set** extend `Dynamic` which builds SQL with `Prepend` ("Where" or "Set").

**For** is more complex — it iterates a collection and builds SQL for each item. It requires `Key`, `Open`, `Close`, `Separator` properties and uses `GetDbProviderPrefix` which needs `context.ExecutionContext.SmartSqlConfig.Database.DbProvider.ParameterPrefix`.

**Important for For tests:** `GetDbProviderPrefix` reads from `context.ExecutionContext`. You need to mock `ExecutionContext` with a `SmartSqlConfig` that has a `Database.DbProvider.ParameterPrefix`. Use Moq:

```csharp
var mockConfig = new Mock<SmartSqlConfig>();
mockConfig.SetupGet(c => c.Database.DbProvider.ParameterPrefix).Returns("@");
mockConfig.SetupGet(c => c.Settings.IgnoreParameterCase).Returns(false);
var mockExecutionContext = new Mock<ExecutionContext>();
mockExecutionContext.SetupGet(e => e.SmartSqlConfig).Returns(mockConfig.Object);
```

However, `SmartSqlConfig` and `ExecutionContext` may not be easily mockable if they lack virtual members or interfaces. Read the actual source files to determine the best approach. If they're not mockable, you may need to construct real instances with minimal setup, or skip the `For` `BuildSql` test and only test `IsCondition`.

- [ ] **Step 1: Create WhereTests**

Test `Where.IsCondition` and `Where.BuildSql`:
- `Should_ReturnTrue_When_ChildTagMatchesCondition`
- `Should_ReturnFalse_When_NoChildTagMatches`
- `Should_PrependWhere_When_BuildingSql`

Create child `SqlText` tags with matching conditions.

- [ ] **Step 2: Create SetTests**

Test `Set.IsCondition` (inherits from `Dynamic`):
- `Should_ReturnTrue_When_ChildTagMatchesCondition`
- `Should_PrependSet_When_BuildingSql`

- [ ] **Step 3: Create DynamicTests**

Test `Dynamic.IsCondition` and `Dynamic.BuildSql`:
- `Should_ReturnTrue_When_AnyChildMatches`
- `Should_ReturnFalse_When_NoChildMatches`
- `Should_Throw_When_MatchedLessThanMin`

- [ ] **Step 4: Create ForTests**

Test `For.IsCondition`:
- `Should_ReturnTrue_When_CollectionIsNonEmpty`
- `Should_ReturnFalse_When_CollectionIsEmpty`
- `Should_ReturnFalse_When_PropertyIsNull`

For `BuildSql` tests, check if `ExecutionContext` can be constructed or mocked. If too complex, only test `IsCondition`.

- [ ] **Step 5: Run tests**

Run: `dotnet test src/SmartSql.Test.Unit/SmartSql.Test.Unit.csproj --filter "FullyQualifiedName~Tags"`
Expected: All tests pass

- [ ] **Step 6: Commit**

```bash
git add -A
git commit -m "test: add Where, Set, Dynamic, For tag tests"
```

---

## Task 7: Add Tags tests — Include, Env, Switch

**Files:**
- Create: `src/SmartSql.Test.Unit/Tags/IncludeTests.cs`
- Create: `src/SmartSql.Test.Unit/Tags/EnvTests.cs`
- Create: `src/SmartSql.Test.Unit/Tags/SwitchTests.cs`

**Env** checks `context.ExecutionContext.SmartSqlConfig.Database.DbProvider.Name` against `DbProvider`. Needs ExecutionContext mock or construction.

**Include** checks child tags for conditions. Test `IsCondition` with child tags.

**Switch** checks `Case` child tags and falls through to `Default`. Test:
- `Should_MatchCase_When_PropertyEqualsCompareValue`
- `Should_MatchDefault_When_NoCaseMatches`

- [ ] **Step 1: Create IncludeTests, EnvTests, SwitchTests**

Follow the same patterns as Task 5-6.

- [ ] **Step 2: Run tests**

Run: `dotnet test src/SmartSql.Test.Unit/SmartSql.Test.Unit.csproj --filter "FullyQualifiedName~Tags"`
Expected: All tests pass

- [ ] **Step 3: Commit**

```bash
git add -A
git commit -m "test: add Include, Env, Switch tag tests"
```

---

## Task 8: Add Cache tests

**Files:**
- Create: `src/SmartSql.Test.Unit/Cache/FifoCacheProviderTests.cs`
- Create: `src/SmartSql.Test.Unit/Cache/LruCacheProviderTests.cs`
- Create: `src/SmartSql.Test.Unit/Cache/CacheKeyTests.cs`

Cache providers are pure in-memory logic with no external dependencies.

- [ ] **Step 1: Create FifoCacheProviderTests**

```csharp
using System.Collections.Generic;
using FluentAssertions;
using SmartSql.Cache;
using SmartSql.Cache.Default;
using Xunit;

namespace SmartSql.Test.Unit.Cache;

public class FifoCacheProviderTests
{
    private FifoCacheProvider CreateProvider(int cacheSize = 3)
    {
        var provider = new FifoCacheProvider();
        provider.Initialize(new Dictionary<string, object> {{"CacheSize", cacheSize}});
        return provider;
    }

    private CacheKey CreateKey(string key)
    {
        return new CacheKey(key, typeof(string));
    }

    [Fact]
    public void Should_AddAndGetItem_When_CacheIsEmpty()
    {
        var provider = CreateProvider();
        var key = CreateKey("key1");
        provider.TryAdd(key, "value1");

        provider.TryGetValue(key, out var value).Should().BeTrue();
        value.Should().Be("value1");
    }

    [Fact]
    public void Should_ReturnFalse_When_KeyNotFound()
    {
        var provider = CreateProvider();

        provider.TryGetValue(CreateKey("missing"), out _).Should().BeFalse();
    }

    [Fact]
    public void Should_ReturnFalse_When_AddingDuplicateKey()
    {
        var provider = CreateProvider();
        var key = CreateKey("key1");
        provider.TryAdd(key, "value1");

        provider.TryAdd(key, "value2").Should().BeFalse();
    }

    [Fact]
    public void Should_EvictOldest_When_CapacityExceeded()
    {
        var provider = CreateProvider(cacheSize: 2);
        var key1 = CreateKey("key1");
        var key2 = CreateKey("key2");
        var key3 = CreateKey("key3");

        provider.TryAdd(key1, "value1");
        provider.TryAdd(key2, "value2");
        provider.TryAdd(key3, "value3");

        provider.TryGetValue(key1, out _).Should().BeFalse();
        provider.TryGetValue(key2, out _).Should().BeTrue();
        provider.TryGetValue(key3, out _).Should().BeTrue();
    }

    [Fact]
    public void Should_ClearAllItems_When_FlushCalled()
    {
        var provider = CreateProvider();
        provider.TryAdd(CreateKey("key1"), "value1");
        provider.TryAdd(CreateKey("key2"), "value2");

        provider.Flush();

        provider.TryGetValue(CreateKey("key1"), out _).Should().BeFalse();
        provider.TryGetValue(CreateKey("key2"), out _).Should().BeFalse();
    }
}
```

- [ ] **Step 2: Create LruCacheProviderTests**

Same structure as FifoCacheProviderTests but test LRU eviction:
- `Should_EvictLeastRecentlyUsed_When_CapacityExceeded`
- After getting `key1`, then adding `key3` should evict `key2` (not `key1`)

```csharp
[Fact]
public void Should_EvictLeastRecentlyUsed_When_CapacityExceeded()
{
    var provider = CreateProvider(cacheSize: 2);
    var key1 = CreateKey("key1");
    var key2 = CreateKey("key2");
    var key3 = CreateKey("key3");

    provider.TryAdd(key1, "value1");
    provider.TryAdd(key2, "value2");

    provider.TryGetValue(key1, out _);

    provider.TryAdd(key3, "value3");

    provider.TryGetValue(key1, out _).Should().BeTrue();
    provider.TryGetValue(key2, out _).Should().BeFalse();
    provider.TryGetValue(key3, out _).Should().BeTrue();
}
```

- [ ] **Step 3: Create CacheKeyTests**

Test `CacheKey` equality:
- `Should_BeEqual_When_SameKeyAndResultType`
- `Should_NotBeEqual_When_DifferentKey`
- `Should_NotBeEqual_When_DifferentResultType`

```csharp
using FluentAssertions;
using SmartSql.Cache;
using Xunit;

namespace SmartSql.Test.Unit.Cache;

public class CacheKeyTests
{
    [Fact]
    public void Should_BeEqual_When_SameKeyAndResultType()
    {
        var key1 = new CacheKey("test-key", typeof(string));
        var key2 = new CacheKey("test-key", typeof(string));

        key1.Equals(key2).Should().BeTrue();
    }

    [Fact]
    public void Should_NotBeEqual_When_DifferentKey()
    {
        var key1 = new CacheKey("key1", typeof(string));
        var key2 = new CacheKey("key2", typeof(string));

        key1.Equals(key2).Should().BeFalse();
    }

    [Fact]
    public void Should_NotBeEqual_When_DifferentResultType()
    {
        var key1 = new CacheKey("test-key", typeof(string));
        var key2 = new CacheKey("test-key", typeof(int));

        key1.Equals(key2).Should().BeFalse();
    }

    [Fact]
    public void Should_HaveSameHashCode_When_SameKey()
    {
        var key1 = new CacheKey("test-key", typeof(string));
        var key2 = new CacheKey("test-key", typeof(string));

        key1.GetHashCode().Should().Be(key2.GetHashCode());
    }
}
```

- [ ] **Step 4: Run tests**

Run: `dotnet test src/SmartSql.Test.Unit/SmartSql.Test.Unit.csproj --filter "FullyQualifiedName~Cache"`
Expected: All tests pass

- [ ] **Step 5: Commit**

```bash
git add -A
git commit -m "test: add FifoCacheProvider, LruCacheProvider, CacheKey tests"
```

---

## Task 9: Add TypeHandler factory tests

**Files:**
- Modify: `src/SmartSql.Test.Unit/TypeHandlers/TypeHandlerFactoryTests.cs` (add new tests)

The existing `TypeHandlerFactoryTest` has 2 tests. Add comprehensive type resolution tests.

- [ ] **Step 1: Add TypeHandlerFactory resolution tests**

Add these tests to the existing `TypeHandlerFactoryTests` class (or the renamed class):

```csharp
[Fact]
public void Should_ResolveStringTypeHandler_When_RequestingStringType()
{
    var factory = new TypeHandlerFactory();

    var handler = factory.GetTypeHandler(typeof(string));

    handler.Should().BeOfType<StringTypeHandler>();
}

[Fact]
public void Should_ResolveInt32TypeHandler_When_RequestingInt32Type()
{
    var factory = new TypeHandlerFactory();

    var handler = factory.GetTypeHandler(typeof(int));

    handler.Should().BeOfType<Int32TypeHandler>();
}

[Fact]
public void Should_ResolveBooleanTypeHandler_When_RequestingBooleanType()
{
    var factory = new TypeHandlerFactory();

    var handler = factory.GetTypeHandler(typeof(bool));

    handler.Should().BeOfType<BooleanTypeHandler>();
}

[Fact]
public void Should_ResolveDateTimeTypeHandler_When_RequestingDateTimeType()
{
    var factory = new TypeHandlerFactory();

    var handler = factory.GetTypeHandler(typeof(DateTime));

    handler.Should().BeOfType<DateTimeTypeHandler>();
}

[Fact]
public void Should_ResolveGuidTypeHandler_When_RequestingGuidType()
{
    var factory = new TypeHandlerFactory();

    var handler = factory.GetTypeHandler(typeof(Guid));

    handler.Should().BeOfType<GuidTypeHandler>();
}

[Fact]
public void Should_ResolveEnumTypeHandler_When_RequestingEnumType()
{
    var factory = new TypeHandlerFactory();

    var handler = factory.GetTypeHandler(typeof(NumericalEnum));

    handler.Should().BeAssignableTo<ITypeHandler>();
}

[Fact]
public void Should_ResolveNullableTypeHandler_When_RequestingNullableType()
{
    var factory = new TypeHandlerFactory();

    var handler = factory.GetTypeHandler(typeof(int?));

    handler.Should().BeOfType<NullableInt32TypeHandler>();
}
```

Add `using SmartSql.Test.Unit.TestEntities;` for `NumericalEnum`.

- [ ] **Step 2: Run tests**

Run: `dotnet test src/SmartSql.Test.Unit/SmartSql.Test.Unit.csproj --filter "FullyQualifiedName~TypeHandlers"`
Expected: All tests pass

- [ ] **Step 3: Commit**

```bash
git add -A
git commit -m "test: add comprehensive TypeHandlerFactory resolution tests"
```

---

## Task 10: Final verification and coverage report

- [ ] **Step 1: Run all unit tests**

Run: `dotnet test src/SmartSql.Test.Unit/SmartSql.Test.Unit.csproj`
Expected: All tests pass, 0 failures

- [ ] **Step 2: Generate coverage report**

Run:
```bash
dotnet test src/SmartSql.Test.Unit/SmartSql.Test.Unit.csproj --collect:"XPlat Code Coverage"
```

- [ ] **Step 3: Verify coverage improvement**

Check that coverage has improved from the baseline (19.4% overall, SmartSql core 20.3%).

- [ ] **Step 4: Verify naming consistency**

```bash
grep -rn "public void Test()" src/SmartSql.Test.Unit/ --include="*.cs"
```

Expected: No matches — all `Test()` methods have been renamed.

```bash
grep -rn "Assert\." src/SmartSql.Test.Unit/ --include="*.cs" | grep -v "MockTypeHandler"
```

Expected: No matches — all `Assert.XXX` replaced with FluentAssertions.

- [ ] **Step 5: Final commit if needed**

```bash
git add -A
git commit -m "refactor: final cleanup after test convention refactoring"
```
