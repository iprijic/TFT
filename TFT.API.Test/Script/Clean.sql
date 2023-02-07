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