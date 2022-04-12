create database SmartSqlTestDB;

use SmartSqlTestDB;

create table if not exists T_AllPrimitive
(
    Id                    bigint       not null primary key,
    Boolean               tinyint(1)   not null,
    `Char`                char         not null,
    Int16                 mediumint    not null,
    Int32                 int          not null,
    Int64                 bigint       not null,
    Single                float        not null,
    `Decimal`             decimal      not null,
    DateTime              datetime     not null,
    String                varchar(100) not null,
    Guid                  char(32)     not null,
    TimeSpan              time         not null,
    NumericalEnum         tinyint(1)   not null,
    NullableBoolean       tinyint(1)   null,
    NullableChar          char         null,
    NullableInt16         mediumint    null,
    NullableInt32         int          null,
    NullableInt64         bigint       null,
    NullableSingle        float        null,
    NullableDecimal       decimal      null,
    NullableDateTime      datetime     null,
    NullableGuid          char(32)     null,
    NullableTimeSpan      time         null,
    NullableNumericalEnum tinyint(1)   null,
    NullableString        varchar(100) null
)
    engine = InnoDB;

