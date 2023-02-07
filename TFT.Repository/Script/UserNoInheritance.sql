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
GO

CREATE TABLE [Actors] (
    [ID] bigint NOT NULL IDENTITY,
    [ActorID] nvarchar(max) NOT NULL,
    [Username] nvarchar(max) NOT NULL,
    [Hash] nvarchar(max) NOT NULL,
    [Salt] nvarchar(max) NOT NULL,
    [Firstname] nvarchar(max) NOT NULL,
    [Lastname] nvarchar(max) NOT NULL,
    [Role] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Actors] PRIMARY KEY ([ID])
);
GO

CREATE TABLE [Directors] (
    [ID] bigint NOT NULL IDENTITY,
    [DirectorID] nvarchar(max) NOT NULL,
    [Username] nvarchar(max) NOT NULL,
    [Hash] nvarchar(max) NOT NULL,
    [Salt] nvarchar(max) NOT NULL,
    [Firstname] nvarchar(max) NOT NULL,
    [Lastname] nvarchar(max) NOT NULL,
    [Role] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Directors] PRIMARY KEY ([ID])
);
GO

CREATE TABLE [Genres] (
    [ID] bigint NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Genres] PRIMARY KEY ([ID])
);
GO

CREATE TABLE [Users] (
    [ID] bigint NOT NULL IDENTITY,
    [Username] nvarchar(max) NOT NULL,
    [Hash] nvarchar(max) NOT NULL,
    [Salt] nvarchar(max) NOT NULL,
    [Firstname] nvarchar(max) NOT NULL,
    [Lastname] nvarchar(max) NOT NULL,
    [Role] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([ID])
);
GO

CREATE TABLE [Movies] (
    [ID] bigint NOT NULL IDENTITY,
    [Title] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Duration] datetimeoffset NOT NULL,
    [StartProduction] datetime NOT NULL,
    [EndProduction] datetime NOT NULL,
    [Budget] decimal(18,0) NOT NULL,
    [DirectorID] bigint NOT NULL,
    CONSTRAINT [PK_Movies] PRIMARY KEY ([ID]),
    CONSTRAINT [FK_MovieDirector] FOREIGN KEY ([DirectorID]) REFERENCES [Directors] ([ID])
);
GO

CREATE TABLE [ActorAgreements] (
    [ID] bigint NOT NULL IDENTITY,
    [IsInvited] bit NOT NULL,
    [IsAccepted] nvarchar(max) NOT NULL,
    [Honorarium] decimal(18,0) NOT NULL,
    [MovieID] bigint NOT NULL,
    [ActorID] bigint NOT NULL,
    CONSTRAINT [PK_ActorAgreements] PRIMARY KEY ([ID]),
    CONSTRAINT [FK_ActorActorAgreement] FOREIGN KEY ([ActorID]) REFERENCES [Actors] ([ID]),
    CONSTRAINT [FK_ActorAgreementMovie] FOREIGN KEY ([MovieID]) REFERENCES [Movies] ([ID])
);
GO

CREATE TABLE [GenreMovie] (
    [Genres_ID] bigint NOT NULL,
    [Movies_ID] bigint NOT NULL,
    CONSTRAINT [PK_GenreMovie] PRIMARY KEY ([Genres_ID], [Movies_ID]),
    CONSTRAINT [FK_GenreMovie_Genre] FOREIGN KEY ([Genres_ID]) REFERENCES [Genres] ([ID]),
    CONSTRAINT [FK_GenreMovie_Movie] FOREIGN KEY ([Movies_ID]) REFERENCES [Movies] ([ID])
);
GO

CREATE INDEX [IX_FK_ActorActorAgreement] ON [ActorAgreements] ([ActorID]);
GO

CREATE INDEX [IX_FK_ActorAgreementMovie] ON [ActorAgreements] ([MovieID]);
GO

CREATE INDEX [IX_FK_GenreMovie_Movie] ON [GenreMovie] ([Movies_ID]);
GO

CREATE INDEX [IX_FK_MovieDirector] ON [Movies] ([DirectorID]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230207151235_InitialMigration', N'6.0.13');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [ActorAgreements] DROP CONSTRAINT [FK_ActorActorAgreement];
GO

ALTER TABLE [Movies] DROP CONSTRAINT [FK_MovieDirector];
GO

DROP TABLE [Actors];
GO

DROP TABLE [Directors];
GO

ALTER TABLE [Users] ADD [ActorID] nvarchar(max) NULL;
GO

ALTER TABLE [Users] ADD [DirectorID] nvarchar(max) NULL;
GO

ALTER TABLE [Users] ADD [Discriminator] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [ActorAgreements] ADD CONSTRAINT [FK_ActorActorAgreement] FOREIGN KEY ([ActorID]) REFERENCES [Users] ([ID]);
GO

ALTER TABLE [Movies] ADD CONSTRAINT [FK_MovieDirector] FOREIGN KEY ([DirectorID]) REFERENCES [Users] ([ID]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230207152422_UserInheritance', N'6.0.13');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [ActorAgreements] DROP CONSTRAINT [FK_ActorActorAgreement];
GO

ALTER TABLE [Movies] DROP CONSTRAINT [FK_MovieDirector];
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'ActorID');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Users] DROP COLUMN [ActorID];
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'DirectorID');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [Users] DROP COLUMN [DirectorID];
GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'Discriminator');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [Users] DROP COLUMN [Discriminator];
GO

CREATE TABLE [Actors] (
    [ID] bigint NOT NULL IDENTITY,
    [ActorID] nvarchar(max) NOT NULL,
    [Username] nvarchar(max) NOT NULL,
    [Hash] nvarchar(max) NOT NULL,
    [Salt] nvarchar(max) NOT NULL,
    [Firstname] nvarchar(max) NOT NULL,
    [Lastname] nvarchar(max) NOT NULL,
    [Role] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Actors] PRIMARY KEY ([ID])
);
GO

CREATE TABLE [Directors] (
    [ID] bigint NOT NULL IDENTITY,
    [DirectorID] nvarchar(max) NOT NULL,
    [Username] nvarchar(max) NOT NULL,
    [Hash] nvarchar(max) NOT NULL,
    [Salt] nvarchar(max) NOT NULL,
    [Firstname] nvarchar(max) NOT NULL,
    [Lastname] nvarchar(max) NOT NULL,
    [Role] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Directors] PRIMARY KEY ([ID])
);
GO

ALTER TABLE [ActorAgreements] ADD CONSTRAINT [FK_ActorActorAgreement] FOREIGN KEY ([ActorID]) REFERENCES [Actors] ([ID]);
GO

ALTER TABLE [Movies] ADD CONSTRAINT [FK_MovieDirector] FOREIGN KEY ([DirectorID]) REFERENCES [Directors] ([ID]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230207160003_UserNoInheritance', N'6.0.13');
GO

COMMIT;
GO

