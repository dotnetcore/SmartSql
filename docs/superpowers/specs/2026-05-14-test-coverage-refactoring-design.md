# Test Coverage & Convention Refactoring Design

## Overview

Refactor all 76 existing unit tests to follow consistent conventions (naming, assertions, structure), then add new tests across 10 modules to raise coverage from 19.4% to a meaningful baseline.

## 1. Refactoring Standards

### Naming Convention

Pattern: `Should_{ExpectedBehavior}_When_{Condition}`

- `Test()` → `Should_RoundTrip_When_EncryptAndDecrypt()`
- `Replace()` → `Should_ReplaceIdPlaceholder_When_SqlContainsId()`
- `GetPropertyValue()` → `Should_ResolveValue_When_KeyExists()`
- Class names: `{ClassUnderTest}Tests` (plural), e.g. `AESCryptoTests`, `PropertiesTests`

### Assertions

Replace all `Assert.XXX` with FluentAssertions:
- `Assert.Equal(expected, actual)` → `actual.Should().Be(expected)`
- `Assert.NotNull(x)` → `x.Should().NotBeNull()`
- `Assert.True(x)` → `x.Should().BeTrue()`
- `Assert.Null(x)` → `x.Should().BeNull()`
- `Assert.Equal(expected, actual)` (types) → `actual.Should().BeOfType<T>()`

### Structure

AAA pattern separated by blank lines, no comments:
```csharp
[Fact]
public void Should_BuildOrderedPipeline_When_AllMiddlewareAdded()
{
    var pipeline = new PipelineBuilder()
        .Add(new ResultHandlerMiddleware())
        .Build();

    pipeline.Should().BeOfType<InitializerMiddleware>();
}
```

## 2. Module-by-Module Plan

### 2.1 Cryptos (3 existing tests)

**Refactor**: Rename classes to plural (`AESCryptoTests`, `DESCryptoTests`, `RSACryptoTests`), rename methods, use FluentAssertions. Uncomment RSACryptoTest and make it pass.

**Add**:
- Should_Throw_When_KeyIsEmpty
- Should_FailDecryption_When_WrongKey
- Should_RoundTrip_When_LargePlainText

### 2.2 ConfigBuilder (9 existing tests)

**Refactor**: `PropertiesTest` → `PropertiesTests`, `XmlConfigLoaderTest` → `XmlConfigLoaderTests`, rename all methods.

**Add**:
- Should_ReturnRawString_When_KeyNotFound
- Should_OverrideValue_When_ImportCalledMultipleTimes

### 2.3 Utils (6 existing tests)

**Refactor**: Rename all test classes and methods. Use FluentAssertions.

**Add**:
- Should_ReturnEmptyList_When_NoParametersFound (SqlParamAnalyzer)
- Should_ParseMultipleParameters_When_SqlHasMany (SqlParamAnalyzer)
- Should_HandleAllTypes_When_Converting (ValueTupleConvert)

### 2.4 PipelineBuilder (1 existing test)

**Refactor**: Replace verbose Assert.Equal chain with FluentAssertions.

**Add**:
- Should_Throw_When_NoMiddlewareAdded
- Should_ReturnSingleMiddleware_When_OnlyOneAdded
- Should_PreserveOrder_When_MiddlewareUnsorted

### 2.5 Reflection (10 existing tests)

**Refactor**: Rename all classes and methods.

**Add**:
- Should_ParseIndexer_When_PropertyHasBracket (PropertyTokenizer)
- Should_ParseNestedProperty_When_PropertyHasDot (PropertyTokenizer)
- Should_Cache_When_SameTypeRequestedTwice (EntityMetaDataCache)

### 2.6 Tags — All New (target > 70%)

No existing tests. Add comprehensive coverage for:
- `SqlText` — parameter expansion, `In` clause generation
- `IsNotEmpty` — empty string / null / collection
- `IsEqual` / `IsGreaterThan` / `IsLessThan` — comparisons
- `Where` — conditional combination, prefix generation
- `Set` — UPDATE SET clause
- `For` — foreach loop expansion
- `Dynamic` — dynamic conditions
- `Include` — SQL fragment references
- `Env` — environment variable checks
- `Switch` — conditional branching

### 2.7 TypeHandlers (4 existing, target > 80%)

**Refactor**: Rename classes and methods.

**Add**:
- All base type registrations (string, int, bool, DateTime, Guid, etc.)
- `EnumTypeHandler` conversion
- `NamedTypeHandlerCache` lookup
- Nullable type handlers

### 2.8 Cache — All New (target > 70%)

Pure in-memory logic:
- `FifoCacheProvider` — eviction, capacity
- `LruCacheProvider` — LRU eviction
- `CacheKey` construction

### 2.9 RequestContext (target > 60%)

**Add**:
- `SetupParameters` — parameter parsing and binding
- `RealSql` direct SQL execution context
- Property assignment

### 2.10 Configuration (target > 50%)

**Add**:
- `SmartSqlConfig` property validation
- `SqlMap` scope parsing
- `Statement` SQL template parsing

## 3. Execution Strategy

Process each module as a single task: refactor existing tests first, then add new tests. Each module produces one commit. Order by dependency: refactor simpler modules first (Cryptos, Utils) before complex ones (Tags, Configuration).
