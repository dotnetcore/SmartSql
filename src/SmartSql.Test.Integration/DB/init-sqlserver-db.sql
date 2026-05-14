IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'T_AllPrimitive')
CREATE TABLE T_AllPrimitive (
    Id                    BIGINT IDENTITY(1,1) PRIMARY KEY,
    Boolean               BIT NOT NULL,
    [Char]                NCHAR(1) NOT NULL,
    Int16                 SMALLINT NOT NULL,
    Int32                 INT NOT NULL,
    Int64                 BIGINT NOT NULL,
    Single                REAL NOT NULL,
    [Decimal]             DECIMAL NOT NULL,
    DateTime              DATETIME2 NOT NULL,
    String                NVARCHAR(100) NOT NULL,
    Guid                  UNIQUEIDENTIFIER NOT NULL,
    TimeSpan              TIME NOT NULL,
    NumericalEnum         SMALLINT NOT NULL,
    NullableBoolean       BIT,
    NullableChar          NCHAR(1),
    NullableInt16         SMALLINT,
    NullableInt32         INT,
    NullableInt64         BIGINT,
    NullableSingle        REAL,
    NullableDecimal       DECIMAL,
    NullableDateTime      DATETIME2,
    NullableGuid          UNIQUEIDENTIFIER,
    NullableTimeSpan      TIME,
    NullableNumericalEnum SMALLINT,
    NullableString        NVARCHAR(100)
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 't_column_annotation_entity')
CREATE TABLE t_column_annotation_entity (
    id          BIGINT IDENTITY(1,1) PRIMARY KEY,
    name        NVARCHAR(100) NOT NULL,
    extend_data NVARCHAR(MAX) NOT NULL
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'T_User')
CREATE TABLE T_User (
    Id       BIGINT IDENTITY(1,1) PRIMARY KEY,
    UserName NVARCHAR(50) NOT NULL,
    Status   TINYINT NOT NULL
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'T_UserExtendedInfo')
CREATE TABLE T_UserExtendedInfo (
    UserId BIGINT IDENTITY(1,1) PRIMARY KEY,
    Data   NVARCHAR(MAX) NOT NULL
);
