create database SmartSqlTestDB;

use SmartSqlTestDB;

create table T_AllPrimitive
(
    Id                    bigint auto_increment primary key,
    Boolean               tinyint(1)   not null,
    `Char`                char         not null,
    Int16                 mediumint    not null,
    Int32                 int          not null,
    Int64                 bigint       not null,
    Single                float        not null,
    `Decimal`             decimal      not null,
    DateTime              datetime     not null,
    String                varchar(100) not null,
    Guid                  char(36)     not null,
    TimeSpan              time         not null,
    NumericalEnum         smallint   not null,
    NullableBoolean       tinyint(1)   null,
    NullableChar          char         null,
    NullableInt16         mediumint    null,
    NullableInt32         int          null,
    NullableInt64         bigint       null,
    NullableSingle        float        null,
    NullableDecimal       decimal      null,
    NullableDateTime      datetime     null,
    NullableGuid          char(36)     null,
    NullableTimeSpan      time         null,
    NullableNumericalEnum smallint   null,
    NullableString        varchar(100) null
) engine = InnoDb;

create table t_column_annotation_entity
(
    id          bigint auto_increment
        primary key,
    name        varchar(100)                 not null,
    extend_data longtext collate utf8mb4_bin not null,
    constraint extend_data
        check (json_valid(`extend_data`))
) engine = InnoDb;

create table T_User
(
    Id       bigint auto_increment
        primary key,
    UserName varchar(50) not null,
    Status   tinyint(2)  not null
) engine = InnoDb;

create table T_UserExtendedInfo
(
    UserId bigint auto_increment
        primary key,
    Data   json not null
) engine = InnoDb;

# Create PROCEDURE SP_Query(out Total int)
# BEGIN
# 
#     Select Count(*)
#     into Total
#     From T_AllPrimitive T;
#     SELECT T.*
#     From T_AllPrimitive T
#     limit 10;
# 
# END;




