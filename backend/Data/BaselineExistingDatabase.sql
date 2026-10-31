-- One-time adoption of a database made by the old EnsureCreated initializer.
-- Select WpfwPortfolioDb in SSMS before running this script. Back up the database first.
-- No application tables or records are changed. A mismatching schema is rejected.
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.__EFMigrationsHistory', N'U') IS NOT NULL
BEGIN
    IF EXISTS (SELECT 1 FROM dbo.__EFMigrationsHistory WHERE MigrationId = N'20261029090740_InitialCreate')
    BEGIN
        PRINT 'InitialCreate is already recorded; no changes made.';
        COMMIT TRANSACTION;
        RETURN;
    END;

    IF EXISTS (SELECT 1 FROM dbo.__EFMigrationsHistory)
        THROW 50001, 'This database has a different migration history. No baseline was applied.', 1;
END;

DECLARE @ExpectedColumns TABLE
(
    TableName sysname,
    ColumnName sysname,
    TypeName sysname,
    MaxLength smallint,
    IsIdentity bit
);

INSERT INTO @ExpectedColumns VALUES
    (N'Projects', N'Id', N'int', 4, 1),
    (N'Projects', N'Title', N'nvarchar', -1, 0),
    (N'Projects', N'Type', N'nvarchar', -1, 0),
    (N'Projects', N'Description', N'nvarchar', -1, 0),
    (N'BlogPosts', N'Id', N'int', 4, 1),
    (N'BlogPosts', N'Title', N'nvarchar', -1, 0),
    (N'BlogPosts', N'Category', N'nvarchar', -1, 0),
    (N'BlogPosts', N'Content', N'nvarchar', -1, 0),
    (N'BlogPosts', N'PublishedOn', N'date', 3, 0);

IF EXISTS
(
    SELECT 1 FROM @ExpectedColumns AS expected
    LEFT JOIN sys.columns AS actual
        ON actual.object_id = OBJECT_ID(N'dbo.' + expected.TableName, N'U')
        AND actual.name = expected.ColumnName
    WHERE actual.column_id IS NULL
        OR TYPE_NAME(actual.user_type_id) <> expected.TypeName
        OR actual.max_length <> expected.MaxLength
        OR actual.is_nullable <> 0
        OR actual.is_identity <> expected.IsIdentity
        OR actual.is_computed <> 0
)
    THROW 50001, 'The database does not match InitialCreate. No baseline was applied.', 1;

IF EXISTS
(
    SELECT 1 FROM sys.columns AS actual
    WHERE actual.object_id IN (OBJECT_ID(N'dbo.Projects'), OBJECT_ID(N'dbo.BlogPosts'))
        AND NOT EXISTS
        (
            SELECT 1 FROM @ExpectedColumns AS expected
            WHERE actual.object_id = OBJECT_ID(N'dbo.' + expected.TableName)
                AND actual.name = expected.ColumnName
        )
)
    THROW 50001, 'Unexpected columns were found. No baseline was applied.', 1;

IF EXISTS
(
    SELECT 1 FROM (VALUES (N'Projects'), (N'BlogPosts')) AS expected(TableName)
    WHERE NOT EXISTS
    (
        SELECT 1 FROM sys.indexes AS pk
        JOIN sys.index_columns AS keyColumn
            ON keyColumn.object_id = pk.object_id AND keyColumn.index_id = pk.index_id
        JOIN sys.columns AS columnInfo
            ON columnInfo.object_id = keyColumn.object_id AND columnInfo.column_id = keyColumn.column_id
        WHERE pk.object_id = OBJECT_ID(N'dbo.' + expected.TableName)
            AND pk.is_primary_key = 1
            AND keyColumn.key_ordinal = 1
            AND columnInfo.name = N'Id'
            AND NOT EXISTS
            (
                SELECT 1 FROM sys.index_columns AS extraKey
                WHERE extraKey.object_id = pk.object_id AND extraKey.index_id = pk.index_id
                    AND extraKey.key_ordinal > 1
            )
    )
)
    THROW 50001, 'Unexpected primary keys were found. No baseline was applied.', 1;

IF OBJECT_ID(N'dbo.__EFMigrationsHistory', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.__EFMigrationsHistory
    (
        MigrationId nvarchar(150) NOT NULL,
        ProductVersion nvarchar(32) NOT NULL,
        CONSTRAINT PK___EFMigrationsHistory PRIMARY KEY (MigrationId)
    );
END;

INSERT INTO dbo.__EFMigrationsHistory (MigrationId, ProductVersion)
VALUES (N'20261029090740_InitialCreate', N'10.0.9');

COMMIT TRANSACTION;
PRINT 'InitialCreate recorded. Existing tables and data have been preserved.';
