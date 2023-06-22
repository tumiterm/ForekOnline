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

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230612112834_UpdateLWM')
BEGIN
    ALTER TABLE [WorkplaceModules] ADD [EndDate] datetime2 NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230612112834_UpdateLWM')
BEGIN
    ALTER TABLE [WorkplaceModules] ADD [StartDate] datetime2 NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230612112834_UpdateLWM')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230612112834_UpdateLWM', N'7.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230615220515_OnlineApps')
BEGIN
    CREATE TABLE [Applications] (
        [ApplicationId] uniqueidentifier NOT NULL,
        [ReferenceNumber] nvarchar(max) NULL,
        [Selection] int NOT NULL,
        [PassportNumber] nvarchar(max) NULL,
        [StudyPermitCategory] int NOT NULL,
        [IDNumber] nvarchar(max) NULL,
        [ApplicantAddressAddressId] uniqueidentifier NOT NULL,
        [ApplicantName] nvarchar(max) NOT NULL,
        [ApplicantSurname] nvarchar(max) NOT NULL,
        [ApplicantTitle] int NOT NULL,
        [Gender] int NOT NULL,
        [HighestQualification] int NOT NULL,
        [IDPassDoc] nvarchar(max) NULL,
        [HighestQualDoc] nvarchar(max) NULL,
        [ResidenceDoc] nvarchar(max) NULL,
        [Status] int NOT NULL,
        [CourseId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Applications] PRIMARY KEY ([ApplicationId]),
        CONSTRAINT [FK_Applications_Address_ApplicantAddressAddressId] FOREIGN KEY ([ApplicantAddressAddressId]) REFERENCES [Address] ([AddressId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230615220515_OnlineApps')
BEGIN
    CREATE TABLE [Medicals] (
        [MedicalId] uniqueidentifier NOT NULL,
        [MedicalName] nvarchar(max) NULL,
        [MemberNumber] nvarchar(max) NULL,
        [Telephone] nvarchar(max) NULL,
        [Disability] nvarchar(max) NULL,
        [ApplicationId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Medicals] PRIMARY KEY ([MedicalId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230615220515_OnlineApps')
BEGIN
    CREATE TABLE [Guardians] (
        [GuardianId] uniqueidentifier NOT NULL,
        [ApplicationId] uniqueidentifier NOT NULL,
        [FirstName] nvarchar(max) NOT NULL,
        [LastName] nvarchar(max) NOT NULL,
        [Relationship] int NOT NULL,
        [Cellphone] nvarchar(max) NOT NULL,
        [IDDoc] nvarchar(max) NULL,
        CONSTRAINT [PK_Guardians] PRIMARY KEY ([GuardianId]),
        CONSTRAINT [FK_Guardians_Applications_ApplicationId] FOREIGN KEY ([ApplicationId]) REFERENCES [Applications] ([ApplicationId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230615220515_OnlineApps')
BEGIN
    CREATE INDEX [IX_Applications_ApplicantAddressAddressId] ON [Applications] ([ApplicantAddressAddressId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230615220515_OnlineApps')
BEGIN
    CREATE UNIQUE INDEX [IX_Guardians_ApplicationId] ON [Guardians] ([ApplicationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230615220515_OnlineApps')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230615220515_OnlineApps', N'7.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230617072917_UpdApplion')
BEGIN
    ALTER TABLE [Applications] ADD [Cellphone] nvarchar(max) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230617072917_UpdApplion')
BEGIN
    ALTER TABLE [Applications] ADD [Email] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230617072917_UpdApplion')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230617072917_UpdApplion', N'7.0.1');
END;
GO

COMMIT;
GO

