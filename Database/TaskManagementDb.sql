/*
    TaskManagementDb.sql
    -----------------------------------------
    Standalone database script for TaskManagement.

    Stack:
      ASP.NET Core 10
      ASP.NET Core Identity
      EF Core / SQL Server
      Task Management

    This script creates:
      - TaskManagementDb
      - ASP.NET Core Identity tables
      - Tasks table
      - Primary keys
      - Indexes
      - Foreign keys
      - EF Core migration history

    NOTE:
      This version intentionally does NOT contain existing users/tasks data.
      A new user can register after the application starts.
*/

USE master;
GO

IF DB_ID(N'TaskManagementDb') IS NULL
BEGIN
    CREATE DATABASE [TaskManagementDb];
END
GO

USE [TaskManagementDb];
GO

/* =========================================================
   EF Core Migration History
   ========================================================= */

IF OBJECT_ID(N'[__EFMigrationsHistory]', N'U') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory]
    (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory]
            PRIMARY KEY ([MigrationId])
    );
END
GO

/* =========================================================
   ASP.NET Core Identity
   ========================================================= */

IF OBJECT_ID(N'[AspNetUsers]', N'U') IS NULL
BEGIN
    CREATE TABLE [AspNetUsers]
    (
        [Id] nvarchar(450) NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,

        CONSTRAINT [PK_AspNetUsers]
            PRIMARY KEY ([Id])
    );
END
GO

IF OBJECT_ID(N'[AspNetUserClaims]', N'U') IS NULL
BEGIN
    CREATE TABLE [AspNetUserClaims]
    (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,

        CONSTRAINT [PK_AspNetUserClaims]
            PRIMARY KEY ([Id]),

        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
            FOREIGN KEY ([UserId])
            REFERENCES [AspNetUsers] ([Id])
            ON DELETE CASCADE
    );
END
GO

/* =========================================================
   Identity Indexes
   ========================================================= */

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_AspNetUserClaims_UserId'
      AND object_id = OBJECT_ID(N'[AspNetUserClaims]')
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId]
        ON [AspNetUserClaims] ([UserId]);
END
GO


IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'UserNameIndex'
      AND object_id = OBJECT_ID(N'[AspNetUsers]')
)
BEGIN
    CREATE UNIQUE INDEX [UserNameIndex]
        ON [AspNetUsers] ([NormalizedUserName])
        WHERE [NormalizedUserName] IS NOT NULL;
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'EmailIndex'
      AND object_id = OBJECT_ID(N'[AspNetUsers]')
)
BEGIN
    CREATE INDEX [EmailIndex]
        ON [AspNetUsers] ([NormalizedEmail]);
END
GO

/* =========================================================
   Tasks
   ========================================================= */

IF OBJECT_ID(N'[Tasks]', N'U') IS NULL
BEGIN
    CREATE TABLE [Tasks]
    (
        [Id] int NOT NULL IDENTITY,
        [Title] nvarchar(200) NOT NULL,
        [Description] nvarchar(max) NULL,
        [CreatedByUserId] nvarchar(450) NOT NULL,
        [AssignedToUserId] nvarchar(450) NULL,
                [CreatedAt] datetime2 NOT NULL
            DEFAULT ((getutcdate())),
        [UpdatedAt] datetime2 NULL,
        [IsDeleted] bit NOT NULL CONSTRAINT [DF_Tasks_IsDeleted] DEFAULT (0),
        [DeletedAt] datetime2 NULL,

        CONSTRAINT [PK_Tasks]
            PRIMARY KEY ([Id]),

        CONSTRAINT [FK_Tasks_AssignedToUser]
            FOREIGN KEY ([AssignedToUserId])
            REFERENCES [AspNetUsers] ([Id]),

        CONSTRAINT [FK_Tasks_CreatedByUser]
            FOREIGN KEY ([CreatedByUserId])
            REFERENCES [AspNetUsers] ([Id])
    );
END
GO

/* =========================================================
   Tasks Indexes
   ========================================================= */

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_Tasks_AssignedToUserId'
      AND object_id = OBJECT_ID(N'[Tasks]')
)
BEGIN
    CREATE INDEX [IX_Tasks_AssignedToUserId]
        ON [Tasks] ([AssignedToUserId]);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_Tasks_CreatedByUserId'
      AND object_id = OBJECT_ID(N'[Tasks]')
)
BEGIN
    CREATE INDEX [IX_Tasks_CreatedByUserId]
        ON [Tasks] ([CreatedByUserId]);
END
GO

/* =========================================================
   Mark the existing EF Core migration as applied.
   This prevents EF from trying to recreate Identity/Tasks
   when the application starts against this database.
   ========================================================= */

IF NOT EXISTS (
    SELECT 1
    FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260926114538_InitialIdentity'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory]
        ([MigrationId], [ProductVersion])
    VALUES
        (N'20260926114538_InitialIdentity', N'10.0.11');
END
GO

PRINT 'TaskManagementDb is ready.';
GO
