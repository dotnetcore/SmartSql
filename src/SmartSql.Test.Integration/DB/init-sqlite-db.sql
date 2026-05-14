CREATE TABLE IF NOT EXISTS T_AllPrimitive (
    Id                    INTEGER PRIMARY KEY AUTOINCREMENT,
    Boolean               INTEGER NOT NULL,
    "Char"                TEXT NOT NULL,
    Int16                 INTEGER NOT NULL,
    Int32                 INTEGER NOT NULL,
    Int64                 INTEGER NOT NULL,
    Single                REAL NOT NULL,
    "Decimal"             REAL NOT NULL,
    DateTime              TEXT NOT NULL,
    String                TEXT NOT NULL,
    Guid                  TEXT NOT NULL,
    TimeSpan              TEXT NOT NULL,
    NumericalEnum         INTEGER NOT NULL,
    NullableBoolean       INTEGER,
    NullableChar          TEXT,
    NullableInt16         INTEGER,
    NullableInt32         INTEGER,
    NullableInt64         INTEGER,
    NullableSingle        REAL,
    NullableDecimal       REAL,
    NullableDateTime      TEXT,
    NullableGuid          TEXT,
    NullableTimeSpan      TEXT,
    NullableNumericalEnum INTEGER,
    NullableString        TEXT
);

CREATE TABLE IF NOT EXISTS t_column_annotation_entity (
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    name        TEXT NOT NULL,
    extend_data TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS T_User (
    Id       INTEGER PRIMARY KEY AUTOINCREMENT,
    UserName TEXT NOT NULL,
    Status   INTEGER NOT NULL
);

CREATE TABLE IF NOT EXISTS T_UserExtendedInfo (
    UserId INTEGER PRIMARY KEY,
    Data   TEXT NOT NULL
);
