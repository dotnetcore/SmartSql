create database if not exists SmartSqlStarterDB;
use SmartSqlStarterDB;

create table if not exists T_AllPrimitive
(
    Id                    bigint       not null AUTO_INCREMENT primary key,
    Boolean               boolean      not null,
    `Char`                char(1)      not null,
    Int16                 mediumint    not null,
    Int32                 int          not null,
    Int64                 bigint       not null,
    Single                float        not null,
    `Decimal`             decimal      not null,
    DateTime              datetime     not null,
    String                varchar(100) not null,
    Guid                  char(36)     not null,
    TimeSpan              time         not null,
    NumericalEnum         tinyint(1)   not null,
    NullableBoolean       boolean,
    NullableChar          char(1),
    NullableInt16         mediumint,
    NullableInt32         int,
    NullableInt64         bigint,
    NullableSingle        float,
    NullableDecimal       decimal,
    NullableDateTime      datetime,
    NullableGuid          char(32),
    NullableTimeSpan      time,
    NullableNumericalEnum tinyint(1),
    NullableString        varchar(100)
) engine = InnoDB;

