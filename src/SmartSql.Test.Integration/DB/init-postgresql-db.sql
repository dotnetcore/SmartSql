CREATE TABLE IF NOT EXISTS "T_AllPrimitive" (
    "Id"                    BIGSERIAL PRIMARY KEY,
    "Boolean"               BOOLEAN NOT NULL,
    "Char"                  CHAR NOT NULL,
    "Int16"                 SMALLINT NOT NULL,
    "Int32"                 INTEGER NOT NULL,
    "Int64"                 BIGINT NOT NULL,
    "Single"                REAL NOT NULL,
    "Decimal"               NUMERIC NOT NULL,
    "DateTime"              TIMESTAMP NOT NULL,
    "String"                VARCHAR(100) NOT NULL,
    "Guid"                  CHAR(36) NOT NULL,
    "TimeSpan"              INTERVAL NOT NULL,
    "NumericalEnum"         SMALLINT NOT NULL,
    "NullableBoolean"       BOOLEAN,
    "NullableChar"          CHAR,
    "NullableInt16"         SMALLINT,
    "NullableInt32"         INTEGER,
    "NullableInt64"         BIGINT,
    "NullableSingle"        REAL,
    "NullableDecimal"       NUMERIC,
    "NullableDateTime"      TIMESTAMP,
    "NullableGuid"          CHAR(36),
    "NullableTimeSpan"      INTERVAL,
    "NullableNumericalEnum" SMALLINT,
    "NullableString"        VARCHAR(100)
);

CREATE TABLE IF NOT EXISTS "t_column_annotation_entity" (
    "id"          BIGSERIAL PRIMARY KEY,
    "name"        VARCHAR(100) NOT NULL,
    "extend_data" JSONB NOT NULL
);

CREATE TABLE IF NOT EXISTS "T_User" (
    "Id"       BIGSERIAL PRIMARY KEY,
    "UserName" VARCHAR(50) NOT NULL,
    "Status"   SMALLINT NOT NULL
);

CREATE TABLE IF NOT EXISTS "T_UserExtendedInfo" (
    "UserId" BIGSERIAL PRIMARY KEY,
    "Data"   JSONB NOT NULL
);
