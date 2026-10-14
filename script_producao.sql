IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Users] (
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(150) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Password] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261003000217_CriaTabelaUsers', N'10.0.12');

COMMIT;
GO

BEGIN TRANSACTION;
DECLARE @var nvarchar(max);
SELECT @var = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'Email');
IF @var IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT ' + @var + ';');
ALTER TABLE [Users] ALTER COLUMN [Email] nvarchar(450) NOT NULL;

CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261003005606_EmailUnico', N'10.0.12');

COMMIT;
GO

BEGIN TRANSACTION;
CREATE TABLE [PreRegisterRequests] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(150) NOT NULL,
    [Email] nvarchar(256) NOT NULL,
    [PasswordHash] nvarchar(255) NOT NULL,
    [VerificationCode] nvarchar(6) NOT NULL,
    [ExpiresAt] datetime2 NOT NULL,
    [AccessFailedCount] int NOT NULL,
    [IsVerified] bit NOT NULL,
    CONSTRAINT [PK_PreRegisterRequests] PRIMARY KEY ([Id])
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261007221930_PreRegisterRequests', N'10.0.12');

COMMIT;
GO

