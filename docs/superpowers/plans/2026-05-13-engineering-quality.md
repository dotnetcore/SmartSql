# Engineering Quality Optimization Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Split SmartSql tests into pure unit tests and integration tests, upgrade CI to .NET 8.0, add code coverage.

**Architecture:** Rename existing `SmartSql.Test.Unit` to `SmartSql.Test.Integration`, create a new `SmartSql.Test.Unit` for pure logic tests. Add separate CI workflows for build/unit-test and integration-test.

**Tech Stack:** .NET 8.0, xUnit 2.9.x, Moq, FluentAssertions, coverlet.collector, ReportGenerator, GitHub Actions

---

## File Structure

### New files
- `src/SmartSql.Test.Unit/SmartSql.Test.Unit.csproj` — new unit test project
- `src/SmartSql.Test.Unit/TestEntities/User.cs` — minimal entity for reflection tests
- `.github/workflows/build.yml` — PR build + unit test + coverage

### Renamed files
- `src/SmartSql.Test.Unit/` → `src/SmartSql.Test.Integration/`
- `src/SmartSql.Test.Unit/SmartSql.Test.Unit.csproj` → `src/SmartSql.Test.Integration/SmartSql.Test.Integration.csproj`
- All `.cs` files in Integration project: namespace `SmartSql.Test.Unit` → `SmartSql.Test.Integration`

### Modified files
- `SmartSql.sln` — update project paths
- `.github/workflows/integration-test.yml` — upgrade Actions + .NET 8.0
- `.github/workflows/package-publish.yml` — upgrade Actions + .NET 8.0

### Migrated files (copy to Unit, delete from Integration)
Batch 1 — 21 independent test files
Batch 2 — 4 type handler tests + 2 tag tests + 3 config/pipeline tests

---

## Task 1: Rename existing test project to Integration

**Files:**
- Rename: `src/SmartSql.Test.Unit/` → `src/SmartSql.Test.Integration/`

- [ ] **Step 1: Rename the directory**

```bash
git mv src/SmartSql.Test.Unit src/SmartSql.Test.Integration
```

- [ ] **Step 2: Rename the project file**

```bash
git mv src/SmartSql.Test.Integration/SmartSql.Test.Unit.csproj src/SmartSql.Test.Integration/SmartSql.Test.Integration.csproj
```

- [ ] **Step 3: Update the csproj — change assembly name and target framework**

In `src/SmartSql.Test.Integration/SmartSql.Test.Integration.csproj`, change:

```xml
<PropertyGroup>
    <IsPackable>false</IsPackable>
    <TargetFramework>net6.0</TargetFramework>
    <RootNamespace>SmartSql.Test.Integration</RootNamespace>
    <AssemblyName>SmartSql.Test.Integration</AssemblyName>
</PropertyGroup>
```

Also upgrade test packages:

```xml
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />
<PackageReference Include="xunit" Version="2.9.3" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.8.2">
```

- [ ] **Step 4: Update namespaces in all .cs files**

```bash
find src/SmartSql.Test.Integration -name "*.cs" -not -path "*/obj/*" -not -path "*/bin/*" -exec sed -i '' 's/namespace SmartSql\.Test\.Unit/namespace SmartSql.Test.Integration/g' {} +
find src/SmartSql.Test.Integration -name "*.cs" -not -path "*/obj/*" -not -path "*/bin/*" -exec sed -i '' 's/using SmartSql\.Test\.Unit/using SmartSql.Test.Integration/g' {} +
```

- [ ] **Step 5: Update SmartSql.sln**

Replace the old project entry. Find the line containing `SmartSql.Test.Unit` and update the path:

```
# Old
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "SmartSql.Test.Unit", "src\SmartSql.Test.Unit\SmartSql.Test.Unit.csproj", "{4A105342-B8C1-4797-B647-2BFF876637F2}"
# New
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "SmartSql.Test.Integration", "src\SmartSql.Test.Integration\SmartSql.Test.Integration.csproj", "{4A105342-B8C1-4797-B647-2BFF876637F2}"
```

Also update the `GlobalSection(ProjectConfigurationPlatforms)` section if it references the path.

Run `dotnet slen SmartSql.sln remove src/SmartSql.Test.Integration/SmartSql.Test.Integration.csproj` then `dotnet sln SmartSql.sln add src/SmartSql.Test.Integration/SmartSql.Test.Integration.csproj` if manual editing is error-prone.

- [ ] **Step 6: Verify the renamed project builds**

Run: `dotnet build src/SmartSql.Test.Integration/SmartSql.Test.Integration.csproj`
Expected: Build succeeds

- [ ] **Step 7: Commit**

```bash
git add -A
git commit -m "refactor: rename SmartSql.Test.Unit to SmartSql.Test.Integration"
```

---

## Task 2: Create new SmartSql.Test.Unit project

**Files:**
- Create: `src/SmartSql.Test.Unit/SmartSql.Test.Unit.csproj`
- Create: `src/SmartSql.Test.Unit/TestEntities/User.cs`

- [ ] **Step 1: Create the project directory and csproj**

```bash
mkdir -p src/SmartSql.Test.Unit/TestEntities
```

Create `src/SmartSql.Test.Unit/SmartSql.Test.Unit.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <IsPackable>false</IsPackable>
    <RootNamespace>SmartSql.Test.Unit</RootNamespace>
    <AssemblyName>SmartSql.Test.Unit</AssemblyName>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />
    <PackageReference Include="xunit" Version="2.9.3" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Moq" Version="4.20.72" />
    <PackageReference Include="FluentAssertions" Version="7.1.0" />
    <PackageReference Include="coverlet.collector" Version="6.0.4">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
    </PackageReference>
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\SmartSql\SmartSql.csproj" />
  </ItemGroup>

</Project>
```

- [ ] **Step 2: Create minimal test entity for reflection tests**

Create `src/SmartSql.Test.Unit/TestEntities/User.cs`:

```csharp
namespace SmartSql.Test.Unit.TestEntities
{
    public class User
    {
        public User() { }

        public User(long id) { Id = id; }

        public User(long id, string name) { Id = id; UserName = name; }

        public virtual long Id { get; set; }
        public virtual string UserName { get; set; }
    }
}
```

- [ ] **Step 3: Add project to solution**

```bash
dotnet sln SmartSql.sln add src/SmartSql.Test.Unit/SmartSql.Test.Unit.csproj
```

Move the solution folder grouping if needed — the new project should be in the same solution folder as the Integration project.

- [ ] **Step 4: Verify the empty project builds**

Run: `dotnet build src/SmartSql.Test.Unit/SmartSql.Test.Unit.csproj`
Expected: Build succeeds

- [ ] **Step 5: Commit**

```bash
git add -A
git commit -m "feat: create new SmartSql.Test.Unit project with Moq, FluentAssertions, coverlet"
```

---

## Task 3: Migrate Batch 1 — Reflection tests (10 files)

**Files:**
- Move from `src/SmartSql.Test.Integration/Reflection/` to `src/SmartSql.Test.Unit/Reflection/`

All 10 files in `Reflection/` are independent (no SmartSqlFixture dependency). Update namespace from `SmartSql.Test.Integration.Reflection` to `SmartSql.Test.Unit.Reflection`. Files that reference `SmartSql.Test.Entities.User` should use `SmartSql.Test.Unit.TestEntities.User` instead.

Files:
- `PropertyTokenizerTest.cs` — no entity references
- `DefaultTypeTest.cs` — check entity references
- `EmitObjectFactoryBuilderTest.cs` — uses `User` entity → update to `TestEntities.User`
- `EntityMetaDataCacheTypeTest.cs` — check entity references
- `EntityProxyCacheFactoryTest.cs` — check entity references
- `ExpressionObjectFactoryBuilderTest.cs` — uses `User` entity → update to `TestEntities.User`
- `GetAccessorFactoryTest.cs` — check entity references
- `ObjectFactoryBuilderTest.cs` — check entity references
- `RequestConvertTest.cs` — check entity references
- `SetAccessorFactoryTest.cs` — check entity references

- [ ] **Step 1: Create directory and move files**

```bash
mkdir -p src/SmartSql.Test.Unit/Reflection
git mv src/SmartSql.Test.Integration/Reflection/*.cs src/SmartSql.Test.Unit/Reflection/
```

- [ ] **Step 2: Update namespaces**

```bash
find src/SmartSql.Test.Unit/Reflection -name "*.cs" -exec sed -i '' 's/namespace SmartSql\.Test\.Integration\.Reflection/namespace SmartSql.Test.Unit.Reflection/g' {} +
find src/SmartSql.Test.Unit/Reflection -name "*.cs" -exec sed -i '' 's/using SmartSql\.Test\.Integration/using SmartSql.Test.Unit/g' {} +
```

- [ ] **Step 3: Update entity references**

For files that reference `SmartSql.Test.Entities.User`, replace with `SmartSql.Test.Unit.TestEntities.User`:

```bash
find src/SmartSql.Test.Unit/Reflection -name "*.cs" -exec sed -i '' 's/using SmartSql\.Test\.Entities;/using SmartSql.Test.Unit.TestEntities;/g' {} +
find src/SmartSql.Test.Unit/Reflection -name "*.cs" -exec sed -i '' 's/SmartSql\.Test\.Entities\./SmartSql.Test.Unit.TestEntities./g' {} +
```

Some files may reference other entities (e.g., `AllPrimitive`, `NumericalEnum`). For those, add the needed entity classes to `TestEntities/` or reference the entity type inline. Inspect each file to determine what's needed.

- [ ] **Step 4: Build and verify**

Run: `dotnet build src/SmartSql.Test.Unit/SmartSql.Test.Unit.csproj`
Expected: Build succeeds. Fix any compilation errors from missing types.

- [ ] **Step 5: Run the unit tests**

Run: `dotnet test src/SmartSql.Test.Unit/SmartSql.Test.Unit.csproj --no-build`
Expected: All migrated tests pass

- [ ] **Step 6: Commit**

```bash
git add -A
git commit -m "refactor: migrate Reflection tests to SmartSql.Test.Unit"
```

---

## Task 4: Migrate Batch 1 — Utils, Cryptos, Attempts tests (10 files)

**Files:**
- Move: `src/SmartSql.Test.Integration/Utils/` → `src/SmartSql.Test.Unit/Utils/` (5 files)
- Move: `src/SmartSql.Test.Integration/Cryptos/` → `src/SmartSql.Test.Unit/Cryptos/` (3 files)
- Move: `src/SmartSql.Test.Integration/Attempts/` → `src/SmartSql.Test.Unit/Attempts/` (2 files)

Utils (5): `WeightFilterTest.cs`, `SqlParamAnalyzerTest.cs`, `TableNameAnalyzerTest.cs`, `ResourceUtilTest.cs`, `ValueTupleConvertTest.cs`, `InsertWithIdTest.cs`
Cryptos (3): `AESCyptoTest.cs`, `DESCryptoTest.cs`, `RSACryptoTest.cs`
Attempts (2): `CreateInstanceFuncTest.cs`, `Tests.cs`

- [ ] **Step 1: Create directories and move files**

```bash
mkdir -p src/SmartSql.Test.Unit/Utils src/SmartSql.Test.Unit/Cryptos src/SmartSql.Test.Unit/Attempts
git mv src/SmartSql.Test.Integration/Utils/*.cs src/SmartSql.Test.Unit/Utils/
git mv src/SmartSql.Test.Integration/Cryptos/*.cs src/SmartSql.Test.Unit/Cryptos/
git mv src/SmartSql.Test.Integration/Attempts/*.cs src/SmartSql.Test.Unit/Attempts/
```

Note: `Attempts/Tests.cs` is in namespace `SmartSql.Test.Integration` (root), not `SmartSql.Test.Integration.Attempts`. Update its namespace to `SmartSql.Test.Unit`.

- [ ] **Step 2: Update namespaces**

```bash
find src/SmartSql.Test.Unit/Utils src/SmartSql.Test.Unit/Cryptos -name "*.cs" -exec sed -i '' 's/namespace SmartSql\.Test\.Integration\./namespace SmartSql.Test.Unit./g' {} +
find src/SmartSql.Test.Unit/Utils src/SmartSql.Test.Unit/Cryptos -name "*.cs" -exec sed -i '' 's/using SmartSql\.Test\.Integration/using SmartSql.Test.Unit/g' {} +
```

For Attempts files, update namespace `SmartSql.Test.Integration` → `SmartSql.Test.Unit` and `SmartSql.Test.Integration.Attempts` → `SmartSql.Test.Unit.Attempts`.

- [ ] **Step 3: Update entity references for Attempts tests**

`CreateInstanceFuncTest.cs` and `Tests.cs` reference `SmartSql.Test.Entities.User`. Replace:

```bash
sed -i '' 's/using SmartSql\.Test\.Entities;/using SmartSql.Test.Unit.TestEntities;/g' src/SmartSql.Test.Unit/Attempts/*.cs
sed -i '' 's/using SmartSql\.Test\.Integration;/using SmartSql.Test.Unit;/g' src/SmartSql.Test.Unit/Attempts/*.cs
```

Check `Utils/InsertWithIdTest.cs` for entity references — if it references `SmartSql.Test.Entities`, add needed entities to `TestEntities/` or update the reference.

- [ ] **Step 4: Build and verify**

Run: `dotnet build src/SmartSql.Test.Unit/SmartSql.Test.Unit.csproj`
Expected: Build succeeds. Fix any compilation errors.

- [ ] **Step 5: Run the unit tests**

Run: `dotnet test src/SmartSql.Test.Unit/SmartSql.Test.Unit.csproj --no-build`
Expected: All tests pass

- [ ] **Step 6: Commit**

```bash
git add -A
git commit -m "refactor: migrate Utils, Cryptos, Attempts tests to SmartSql.Test.Unit"
```

---

## Task 5: Migrate Batch 2 — Tags, TypeHandlers, ConfigBuilder, PipelineBuilder (8 files)

**Files:**
- Move: `ScriptTest.cs`, `SqlTextTest.cs` from `Tags/`
- Move: `TypeHandlerFactoryTest.cs`, `Int64TypeHandlerTest.cs`, `NamedTypeHandlerCacheTest.cs`, `MockTypeHandlerDbDataReader.cs` from `TypeHandlers/`
- Move: `XmlConfigLoaderTest.cs`, `PropertiesTest.cs` from `ConfigBuilder/`
- Move: `PipelineBuilderTest.cs` from root

**Important:** `XmlConfigLoaderTest.cs` references `SmartSqlMapConfig.xml` config file. This file needs to be either:
- Copied to the Unit project, or
- Left in Integration and the test kept there

Check: if it only loads XML from a file path and parses config without DB, it can be migrated by copying the XML file.

- [ ] **Step 1: Create directories and move files**

```bash
mkdir -p src/SmartSql.Test.Unit/Tags src/SmartSql.Test.Unit/TypeHandlers src/SmartSql.Test.Unit/ConfigBuilder
git mv src/SmartSql.Test.Integration/Tags/ScriptTest.cs src/SmartSql.Test.Unit/Tags/
git mv src/SmartSql.Test.Integration/Tags/SqlTextTest.cs src/SmartSql.Test.Unit/Tags/
git mv src/SmartSql.Test.Integration/TypeHandlers/TypeHandlerFactoryTest.cs src/SmartSql.Test.Unit/TypeHandlers/
git mv src/SmartSql.Test.Integration/TypeHandlers/Int64TypeHandlerTest.cs src/SmartSql.Test.Unit/TypeHandlers/
git mv src/SmartSql.Test.Integration/TypeHandlers/NamedTypeHandlerCacheTest.cs src/SmartSql.Test.Unit/TypeHandlers/
git mv src/SmartSql.Test.Integration/TypeHandlers/MockTypeHandlerDbDataReader.cs src/SmartSql.Test.Unit/TypeHandlers/
git mv src/SmartSql.Test.Integration/ConfigBuilder/XmlConfigLoaderTest.cs src/SmartSql.Test.Unit/ConfigBuilder/
git mv src/SmartSql.Test.Integration/ConfigBuilder/PropertiesTest.cs src/SmartSql.Test.Unit/ConfigBuilder/
git mv src/SmartSql.Test.Integration/PipelineBuilderTest.cs src/SmartSql.Test.Unit/
```

- [ ] **Step 2: Copy config files needed by XmlConfigLoaderTest**

```bash
cp src/SmartSql.Test.Integration/SmartSqlMapConfig.xml src/SmartSql.Test.Unit/
```

Add to `SmartSql.Test.Unit.csproj`:

```xml
<ItemGroup>
  <None Update="SmartSqlMapConfig.xml">
    <CopyToOutputDirectory>Always</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

- [ ] **Step 3: Update namespaces in all moved files**

```bash
find src/SmartSql.Test.Unit/Tags src/SmartSql.Test.Unit/TypeHandlers src/SmartSql.Test.Unit/ConfigBuilder src/SmartSql.Test.Unit/PipelineBuilderTest.cs -name "*.cs" -exec sed -i '' 's/namespace SmartSql\.Test\.Integration/namespace SmartSql.Test.Unit/g' {} +
find src/SmartSql.Test.Unit/Tags src/SmartSql.Test.Unit/TypeHandlers src/SmartSql.Test.Unit/ConfigBuilder src/SmartSql.Test.Unit/PipelineBuilderTest.cs -name "*.cs" -exec sed -i '' 's/using SmartSql\.Test\.Integration/using SmartSql.Test.Unit/g' {} +
```

- [ ] **Step 4: Update entity references in TypeHandler files**

`Int64TypeHandlerTest.cs` references `SmartSql.Test.Entities`. Check what entities it uses and update:

```bash
grep -l "SmartSql.Test.Entities" src/SmartSql.Test.Unit/TypeHandlers/*.cs
```

Replace entity references as needed, similar to Task 3.

- [ ] **Step 5: Build and verify**

Run: `dotnet build src/SmartSql.Test.Unit/SmartSql.Test.Unit.csproj`
Expected: Build succeeds. Fix any compilation errors.

- [ ] **Step 6: Run the unit tests**

Run: `dotnet test src/SmartSql.Test.Unit/SmartSql.Test.Unit.csproj --no-build`
Expected: All tests pass

- [ ] **Step 7: Commit**

```bash
git add -A
git commit -m "refactor: migrate Tags, TypeHandlers, ConfigBuilder, PipelineBuilder tests to SmartSql.Test.Unit"
```

---

## Task 6: Verify Integration project still builds and passes

- [ ] **Step 1: Build Integration project**

Run: `dotnet build src/SmartSql.Test.Integration/SmartSql.Test.Integration.csproj`
Expected: Build succeeds. If any moved files left stale references, fix them.

- [ ] **Step 2: Clean up empty directories in Integration**

```bash
find src/SmartSql.Test.Integration -type d -empty -delete
```

- [ ] **Step 3: Build full solution**

Run: `dotnet build SmartSql.sln`
Expected: Build succeeds for all projects

- [ ] **Step 4: Commit any cleanup**

```bash
git add -A
git commit -m "refactor: clean up Integration project after migration"
```

---

## Task 7: Add CI workflow — build.yml

**Files:**
- Create: `.github/workflows/build.yml`

- [ ] **Step 1: Create build.yml**

Create `.github/workflows/build.yml`:

```yaml
name: Build & Unit Test
on:
  push:
    branches: [ master, refactor ]
  pull_request:
    branches: [ master ]

jobs:
  build:
    name: Build & Unit Test
    runs-on: ubuntu-latest

    steps:
      - name: Checkout
        uses: actions/checkout@v4

      - name: Set up .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: 8.0.x

      - name: Restore dependencies
        run: dotnet restore SmartSql.sln

      - name: Build
        run: dotnet build SmartSql.sln --no-restore --configuration Release

      - name: Run Unit Tests
        run: dotnet test src/SmartSql.Test.Unit/SmartSql.Test.Unit.csproj --no-build --configuration Release --collect:"XPlat Code Coverage" --results-directory ./coverage

      - name: Generate Coverage Report
        run: |
          dotnet tool install --global dotnet-reportgenerator-globaltool --version 5.* || true
          reportgenerator -reports:"./coverage/**/coverage.cobertura.xml" -targetdir:"./coverage/report" -reporttypes:"Html;Cobertura;TextSummary"

      - name: Print Coverage Summary
        run: cat ./coverage/report/Summary.txt

      - name: Upload Coverage Report
        uses: actions/upload-artifact@v4
        if: always()
        with:
          name: coverage-report
          path: ./coverage/report/

      - name: Upload to Codecov
        uses: codecov/codecov-action@v4
        with:
          files: ./coverage/**/coverage.cobertura.xml
          token: ${{ secrets.CODECOV_TOKEN }}
```

- [ ] **Step 2: Commit**

```bash
git add .github/workflows/build.yml
git commit -m "ci: add build and unit test workflow with code coverage"
```

---

## Task 8: Upgrade integration-test.yml

**Files:**
- Modify: `.github/workflows/integration-test.yml`

- [ ] **Step 1: Update integration-test.yml**

Replace the entire file with:

```yaml
name: Integration Test
on:
  push:
    branches: [ master, refactor ]
  pull_request:
    branches: [ master ]

jobs:
  integration-test:
    name: Integration Test
    runs-on: ubuntu-latest
    env:
      REDIS: true

    services:
      redis:
        image: redis
        options: >-
          --health-cmd "redis-cli ping"
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5
        ports:
          - 6379:6379

    steps:
      - name: Start MySQL
        run: sudo /etc/init.d/mysql start

      - name: Checkout
        uses: actions/checkout@v4

      - name: Set up .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: 8.0.x

      - name: Init SmartSql-Test-Db
        run: mysql -vvv -h localhost -uroot -proot < src/SmartSql.Test.Integration/DB/init-mysql-db.sql

      - name: Run Integration Tests
        run: dotnet test src/SmartSql.Test.Integration/SmartSql.Test.Integration.csproj
```

Key changes: `checkout@v4`, `setup-dotnet@v4`, `net8.0.x`, test path updated, added `refactor` branch trigger.

- [ ] **Step 2: Commit**

```bash
git add .github/workflows/integration-test.yml
git commit -m "ci: upgrade integration-test workflow to .NET 8.0 and Actions v4"
```

---

## Task 9: Upgrade package-publish.yml

**Files:**
- Modify: `.github/workflows/package-publish.yml`

- [ ] **Step 1: Update package-publish.yml**

Replace the entire file with:

```yaml
name: Packages Publish
on:
  release:
    types: [created]

jobs:
  nuget-publish:
    runs-on: ubuntu-latest
    permissions:
      contents: read
      packages: write
    steps:
      - name: Checkout
        uses: actions/checkout@v4

      - name: Set up .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: 8.0.x

      - name: Pack
        run: dotnet pack -c Release -o ./nuget

      - name: Publish
        run: dotnet nuget push "./nuget/*.nupkg" -s https://api.nuget.org/v3/index.json -k ${{ secrets.NUGET_API_KEY }}
```

- [ ] **Step 2: Commit**

```bash
git add .github/workflows/package-publish.yml
git commit -m "ci: upgrade package-publish workflow to .NET 8.0 and Actions v4"
```

---

## Task 10: Final verification

- [ ] **Step 1: Build the entire solution**

Run: `dotnet build SmartSql.sln`
Expected: All projects build successfully

- [ ] **Step 2: Run unit tests with coverage**

Run: `dotnet test src/SmartSql.Test.Unit/SmartSql.Test.Unit.csproj --collect:"XPlat Code Coverage"`
Expected: All tests pass, coverage data generated

- [ ] **Step 3: Verify no stale references**

```bash
grep -r "SmartSql.Test.Unit" src/SmartSql.Test.Integration/ --include="*.cs" -l
```

Expected: No matches (all references to old unit test namespace have been cleaned up)

- [ ] **Step 4: Verify solution structure**

```bash
dotnet sln SmartSql.sln list
```

Expected: Shows both `SmartSql.Test.Unit` and `SmartSql.Test.Integration`

- [ ] **Step 5: Final commit if any cleanup needed**

```bash
git add -A
git commit -m "refactor: final cleanup after test reorganization"
```
