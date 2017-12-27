# SmartSql - [Documentation](https://doc.smartsql.net/)
## 0. Why ? 
 - Embrace the cross platform. DotNet Core, it's time！ 
 - Based on Dapper, no more wheels are repeated. Dapper performance you know! 
 ----
## 1. So SmartSql
 - TargetFramework: .NETStandard,Version=v1.3 
 - SmartSql = Dapper + MyBatis + Cache(Memory | Redis) + ZooKeeper + R/W Splitting + ...... 
----
## 2. Feature 
- 1 ORM
  - 1.1 Sync
  - 1.2 Async
- 2 XmlConfig & XmlStatement -> Sql
  - 2.1 SmartSqlMapConfig & SmartSqlMap √  (Yes, you guessed that, like MyBatis, you separated SQL from the XML configuration.)
  - 2.2 Config Hot Update ->ConfigWatcher & Reload (Configuration file hot update: when you need to change Sql, modify the SqlMap configuration file directly and save it.)
- 3 Read-write separation
  - 3.1 Read-write separation
  - 3.2 Election of the read database by weight
  - 3.3 ~~读库故障检测,剔除~~
- 4 Logging √
  - 4.1 Base on Microsoft.Extensions.Logging.Abstractions  (When you need to track the debugging when everything is so clear at a glance)
- 5 DAO
  - 5.1 DAO
  - 5.2 DAO Tool
    - 5.2.1 Template Xml & Entity & DAO
    - 5.2.2 Generate Tool
- 6 Query Cache
  - 6.1 SmartSql.Cache.Memory
      - 6.1.1 Fifo 
      - 6.1.2 Lru 
  - 6.2 SmartSql.Cache.Redis
  - 6.3 Cache transaction consistency
- 7 Distributed configuration plugin 
  - 7.1 IConfigLoader
  - 7.2 LocalFileConfigLoader  √ (Local file configuration loader)
      - 7.2.1 Load SmartSqlMapSource Xml  √
      - 7.3.1 Load SmartSqlMapSource Directory √
  - 7.3 SmartSql.ZooKeeperConfig √ (Distributed configuration file loader by ZooKeeper)
----
## 3. Performance 
### Query Times:1000000 

| ORM | Total\(ms\) |
| --- | :---: |
| SmartSql | 63568 |
| Dapper | 60023 |
| MyBaits | 83566 |

### Query Times:100000

| ORM | Total\(ms\) |
| --- | :---: |
| SmartSql | 6075 |
| Dapper | 5931 |
| MyBaits | 6574 |
----
## 4. Configuration 

### 4.1 SmartSqlMapConfig 
``` xml
<?xml version="1.0" encoding="utf-8" ?>
<SmartSqlMapConfig xmlns="http://SmartSql.net/schemas/SmartSqlMapConfig.xsd">
  <Settings
    IsWatchConfigFile="true"

  />
  <Database>
    <!--<DbProvider Name="MySqlClientFactory" ParameterPrefix="?" Type="MySql.Data.MySqlClient.MySqlClientFactory,MySql.Data"/>-->
    <DbProvider Name="SqlClientFactory" ParameterPrefix="@" Type="System.Data.SqlClient.SqlClientFactory,System.Data.SqlClient"/>
    <Write Name="WriteDB" ConnectionString="Data Source=.;database=TestDB;uid=sa;pwd=SmartSql.net"/>
    <Read Name="ReadDB-0" ConnectionString="Data Source=.;database=TestDB;uid=sa;pwd=SmartSql.net" Weight="80"/>
    <Read Name="ReadDB-1" ConnectionString="Data Source=.;database=TestDB;uid=sa;pwd=SmartSql.net" Weight="20"/>
  </Database>
  <SmartSqlMaps>
    <SmartSqlMap Path="Maps" Type="Directory"></SmartSqlMap>
  </SmartSqlMaps>
</SmartSqlMapConfig>
```
### 4.2 SmartSqlMap 
``` xml
<?xml version="1.0" encoding="utf-8" ?>
<SmartSqlMap Scope="T_Test"  xmlns="http://SmartSql.net/schemas/SmartSqlMap.xsd">
  <Statements>
    <Statement Id="QueryParams">
      
    </Statement>
    <!--Insert-->
    <Statement Id="Insert">
      INSERT INTO T_Test
      (Name)
      VALUES
      (@Name)
      ;Select Scope_Identity();
    </Statement>
    <!--Delete-->
    <Statement Id="Delete">
      Delete T_Test
      Where Id=@Id
    </Statement>
    <!--Update-->
    <Statement Id="Update">
      UPDATE T_Test
      SET
      Name = @Name
      Where Id=@Id
    </Statement>
    <!--GetList-->
    <Statement Id="GetList">
      SELECT T.* From T_Test T With(NoLock)
      <Include RefId="QueryParams"/>
      Order By T.Id Desc
    </Statement>
    <!--GetListByPage-->
    <Statement Id="GetListByPage">
      Select TT.* From
      (Select ROW_NUMBER() Over(Order By T.Id Desc) Row_Index,T.* From T_Test T With(NoLock)
      <Include RefId="QueryParams"/>) TT
      Where TT.Row_Index Between ((@PageIndex-1)*@PageSize+1) And (@PageIndex*@PageSize)
    </Statement>
    <!--GetRecord-->
    <Statement Id="GetRecord">
      Select Count(1) From T_Test T With(NoLock)
      <Include RefId="QueryParams"/>
    </Statement>
    <!--GetEntity-->
    <Statement Id="GetEntity">
      Select Top 1 T.* From T_Test T With(NoLock)
      <Where>
        <IsNotEmpty Prepend="And" Property="Id">
          T.Id=@Id
        </IsNotEmpty>
      </Where>
    </Statement>
    <!--IsExist-->
    <Statement Id="IsExist">
      Select Count(1) From T_Test T With(NoLock)
      <Include RefId="QueryParams"/>
    </Statement>
  </Statements>
</SmartSqlMap>
```
## Install (NuGet)
```
Install-Package SmartSql
```
## Codes
### Query
``` CSharp
            ISmartSqlMapper SqlMapper = MapperContainer.Instance.GetSqlMapper();
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
                ISmartSqlMapper SqlMapper = MapperContainer.Instance.GetSqlMapper();
                SqlMapper.BeginTransaction();
                SqlMapper.Execute(new RequestContext
                {
                    Scope = "T_Test",
                    SqlId = "Add",
                    Request = new T_Test { }
                });
                SqlMapper.Execute(new RequestContext
                {
                    Scope = "T_Test",
                    SqlId = "Update",
                    Request = new T_Test { }
                });
                SqlMapper.CommitTransaction();
            }
            catch (Exception ex)
            {
                SqlMapper.RollbackTransaction();
                throw ex;
            }
```
##  Technology exchange group 
- QQ group Id : 604762592 