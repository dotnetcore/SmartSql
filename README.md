# SmartSql
## 0. Why ?
 - MyBaits.Net 不再更新维护。
 - 拥抱 跨平台、.NET Core，是时候了。 
 - 基于Dapper，不再重复造轮子。Dapper性能你懂的。

## 1. So SmartSql
 - SmartSql = Dapper + MyBatis 
 - TargetFramework: .NETStandard,Version=v1.4

## 2. 主要特性 (√ 为已完成，未打 √ 为计划特性)
- 1 ORM 
  - 1.1 Sync √
  - 1.2 Async √
- 2 XmlConfig & XmlStatement -> Sql  √
  - 2.1 SmartSqlMapConfig & SmartSqlMap √  (是的，你猜对了，和MyBaits一样，通过XML配置分离SQL。)
  - 2.2 Config Hot Update ->ConfigWatcher & Reload √ (配置文件热更新：当你需要修改Sql的时候，直接修改SqlMap配置文件，保存即可。)
- 3 读写分离 √
  - 3.1 读写分离 √ 
  - 3.2 多读库 权重筛选 √ （配置多读库，根据读库权重选举读库）
- 4 Logging √
  - 4.1 NLog √ （当你需要跟踪调试的时候一切都是那么一目了然）
- 5 DAO
  - 5.1 DAO
  - 5.2 DAO Tool 
    - 5.2.1 Template Xml & Entity & DAO
    - 5.2.2 Generate Tool
- 6 Query Cache
  - 6.1 SmartSql.Cache.Memory
  - 6.2 SmartSql.Cache.Redis
- 7 分布式事务
- 8 主从同步

## 3. 配置

### 3.1 SmartSqlMapConfig
``` Xml
<?xml version="1.0" encoding="utf-8" ?>
<SmartSqlMapConfig xmlns="http://SmartSql.net/schemas/SmartSqlMapConfig.xsd">
  <Settings
    IsWatchConfigFile="false"
  />
  <Database>
    <DbProvider Name="SqlClientFactory" ParameterPrefix="@" Type="System.Data.SqlClient.SqlClientFactory,System.Data.SqlClient"/>
    <Write Name="WriteDB" ConnectionString="Data Source=.;database=TestDB;uid=sa;pwd=SmartSql.net"/>
    <Read Name="ReadDB-0" ConnectionString="Data Source=.;database=TestDB;uid=sa;pwd=SmartSql.net" Weight="80"/>
    <Read Name="ReadDB-1" ConnectionString="Data Source=.;database=TestDB;uid=sa;pwd=SmartSql.net" Weight="20"/>
  </Database>
  <SmartSqlMaps>
    <SmartSqlMap Path="Maps/T_Test.xml"></SmartSqlMap>
    
  </SmartSqlMaps>
</SmartSqlMapConfig>
``` 
### 3.2 Logging
``` xml
<?xml version="1.0" encoding="utf-8" ?>
<SmartSqlLog xmlns="http://SmartSql.net/schemas/SmartSqlLog.xsd">
  <LoggerFactoryAdapter Name="NLoggerFactoryAdapter" Type="SmartSql.Logging.Impl.NLoggerFactoryAdapter,SmartSql.Logging.NLog"/>
</SmartSqlLog>
```

### 3.3 SmartSqlMap
``` xml
<?xml version="1.0" encoding="utf-8" ?>
<SmartSqlMap Scope="T_Test"  xmlns="http://SmartSql.net/schemas/SmartSqlMap.xsd">
  <Statements>
    <Statement Id="GetList">
      Select Top 10 * From T_Test Where 1=1

      <IsNotEmpty Prepend="And Id" Property="Ids" In="true">
      </IsNotEmpty>
      <IsEmpty Prepend="And" Property="Id">
      </IsEmpty>
      <IsEqual Prepend="And" Property="Id" CompareValue="0">Id=@Id</IsEqual>
      <IsGreaterEqual Prepend="" Property="" CompareValue=""></IsGreaterEqual>
      <IsGreaterThan Prepend="" Property="" CompareValue=""></IsGreaterThan>
      <IsLessEqual Prepend="" Property="" CompareValue=""></IsLessEqual>
      <IsLessThan Prepend="" Property="" CompareValue=""></IsLessThan>
      <IsNotEmpty Prepend="" Property="">
      </IsNotEmpty>
      <IsNotEqual Prepend="" Property="" CompareValue=""></IsNotEqual>
      <IsNotNull Prepend="" Property=""></IsNotNull>
      <IsNull Prepend="" Property=""></IsNull>
      Order By Id Desc
    </Statement>
    <Statement Id="Insert">
      INSERT INTO T_Test
      ([Name])
      VALUES
      (@Name)
      ;
      Select @@IDENTITY
    </Statement>
    
  </Statements>
</SmartSqlMap>

```
## 安装 (NuGet)
```
Install-Package SmartSql

Install-Package SmartSql.Logging.NLog
```
## Codes
### Query
``` CSharp
            ISmartSqlMapper SqlMapper = new SmartSqlMapper();
            SqlMapper.Query<T_Test>(new RequestContext
            {
                Scope = "T_Test",
                SqlId = "GetList",
                Request = new { Ids = new long[] { 1, 2, 3, 4 } }
            });
```
### Transaction
``` CSharp
            try
            {
                sqlMap.BeginTransaction();
                sqlMap.Execute(new RequestContext
                {
                    Scope = "T_Test",
                    SqlId = "Add",
                    Request = new T_Test { }
                });
                sqlMap.Execute(new RequestContext
                {
                    Scope = "T_Test",
                    SqlId = "Update",
                    Request = new T_Test { }
                });
                sqlMap.CommitTransaction();
            }
            catch (Exception ex)
            {
                sqlMap.RollbackTransaction();
                throw ex;
            }
```