
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 02/07/2023 01:53:55
-- Generated from EDMX file: C:\Programming Projects\SVAM Plus\Repository\Repository4\TFTModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [TFT.Test];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_ActorAgreementMovie]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ActorAgreements] DROP CONSTRAINT [FK_ActorAgreementMovie];
GO
IF OBJECT_ID(N'[dbo].[FK_GenreMovie_Genre]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GenreMovie] DROP CONSTRAINT [FK_GenreMovie_Genre];
GO
IF OBJECT_ID(N'[dbo].[FK_GenreMovie_Movie]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GenreMovie] DROP CONSTRAINT [FK_GenreMovie_Movie];
GO
IF OBJECT_ID(N'[dbo].[FK_ActorActorAgreement]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ActorAgreements] DROP CONSTRAINT [FK_ActorActorAgreement];
GO
IF OBJECT_ID(N'[dbo].[FK_MovieDirector]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Movies] DROP CONSTRAINT [FK_MovieDirector];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Genres]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Genres];
GO
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO
IF OBJECT_ID(N'[dbo].[Movies]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Movies];
GO
IF OBJECT_ID(N'[dbo].[ActorAgreements]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ActorAgreements];
GO
IF OBJECT_ID(N'[dbo].[Actors]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Actors];
GO
IF OBJECT_ID(N'[dbo].[Directors]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Directors];
GO
IF OBJECT_ID(N'[dbo].[GenreMovie]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GenreMovie];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Genres'
CREATE TABLE [dbo].[Genres] (
    [ID] bigint IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [ID] bigint IDENTITY(1,1) NOT NULL,
    [Username] nvarchar(max)  NOT NULL,
    [Hash] nvarchar(max)  NOT NULL,
    [Salt] nvarchar(max)  NOT NULL,
    [Firstname] nvarchar(max)  NOT NULL,
    [Lastname] nvarchar(max)  NOT NULL,
    [Role] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Movies'
CREATE TABLE [dbo].[Movies] (
    [ID] bigint IDENTITY(1,1) NOT NULL,
    [Title] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [Duration] datetimeoffset  NOT NULL,
    [StartProduction] datetime  NOT NULL,
    [EndProduction] datetime  NOT NULL,
    [Budget] decimal(18,0)  NOT NULL,
    [DirectorID] bigint  NOT NULL
);
GO

-- Creating table 'ActorAgreements'
CREATE TABLE [dbo].[ActorAgreements] (
    [ID] bigint IDENTITY(1,1) NOT NULL,
    [IsInvited] bit  NOT NULL,
    [IsAccepted] nvarchar(max)  NOT NULL,
    [Honorarium] decimal(18,0)  NOT NULL,
    [MovieID] bigint  NOT NULL,
    [ActorID] bigint  NOT NULL
);
GO

-- Creating table 'Actors'
CREATE TABLE [dbo].[Actors] (
    [ID] bigint IDENTITY(1,1) NOT NULL,
    [ActorID] nvarchar(max)  NOT NULL,
    [Username] nvarchar(max)  NOT NULL,
    [Hash] nvarchar(max)  NOT NULL,
    [Salt] nvarchar(max)  NOT NULL,
    [Firstname] nvarchar(max)  NOT NULL,
    [Lastname] nvarchar(max)  NOT NULL,
    [Role] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Directors'
CREATE TABLE [dbo].[Directors] (
    [ID] bigint IDENTITY(1,1) NOT NULL,
    [DirectorID] nvarchar(max)  NOT NULL,
    [Username] nvarchar(max)  NOT NULL,
    [Hash] nvarchar(max)  NOT NULL,
    [Salt] nvarchar(max)  NOT NULL,
    [Firstname] nvarchar(max)  NOT NULL,
    [Lastname] nvarchar(max)  NOT NULL,
    [Role] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'GenreMovie'
CREATE TABLE [dbo].[GenreMovie] (
    [Genres_ID] bigint  NOT NULL,
    [Movies_ID] bigint  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ID] in table 'Genres'
ALTER TABLE [dbo].[Genres]
ADD CONSTRAINT [PK_Genres]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Movies'
ALTER TABLE [dbo].[Movies]
ADD CONSTRAINT [PK_Movies]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'ActorAgreements'
ALTER TABLE [dbo].[ActorAgreements]
ADD CONSTRAINT [PK_ActorAgreements]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Actors'
ALTER TABLE [dbo].[Actors]
ADD CONSTRAINT [PK_Actors]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Directors'
ALTER TABLE [dbo].[Directors]
ADD CONSTRAINT [PK_Directors]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [Genres_ID], [Movies_ID] in table 'GenreMovie'
ALTER TABLE [dbo].[GenreMovie]
ADD CONSTRAINT [PK_GenreMovie]
    PRIMARY KEY CLUSTERED ([Genres_ID], [Movies_ID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [MovieID] in table 'ActorAgreements'
ALTER TABLE [dbo].[ActorAgreements]
ADD CONSTRAINT [FK_ActorAgreementMovie]
    FOREIGN KEY ([MovieID])
    REFERENCES [dbo].[Movies]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ActorAgreementMovie'
CREATE INDEX [IX_FK_ActorAgreementMovie]
ON [dbo].[ActorAgreements]
    ([MovieID]);
GO

-- Creating foreign key on [Genres_ID] in table 'GenreMovie'
ALTER TABLE [dbo].[GenreMovie]
ADD CONSTRAINT [FK_GenreMovie_Genre]
    FOREIGN KEY ([Genres_ID])
    REFERENCES [dbo].[Genres]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Movies_ID] in table 'GenreMovie'
ALTER TABLE [dbo].[GenreMovie]
ADD CONSTRAINT [FK_GenreMovie_Movie]
    FOREIGN KEY ([Movies_ID])
    REFERENCES [dbo].[Movies]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_GenreMovie_Movie'
CREATE INDEX [IX_FK_GenreMovie_Movie]
ON [dbo].[GenreMovie]
    ([Movies_ID]);
GO

-- Creating foreign key on [ActorID] in table 'ActorAgreements'
ALTER TABLE [dbo].[ActorAgreements]
ADD CONSTRAINT [FK_ActorActorAgreement]
    FOREIGN KEY ([ActorID])
    REFERENCES [dbo].[Actors]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ActorActorAgreement'
CREATE INDEX [IX_FK_ActorActorAgreement]
ON [dbo].[ActorAgreements]
    ([ActorID]);
GO

-- Creating foreign key on [DirectorID] in table 'Movies'
ALTER TABLE [dbo].[Movies]
ADD CONSTRAINT [FK_MovieDirector]
    FOREIGN KEY ([DirectorID])
    REFERENCES [dbo].[Directors]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MovieDirector'
CREATE INDEX [IX_FK_MovieDirector]
ON [dbo].[Movies]
    ([DirectorID]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------