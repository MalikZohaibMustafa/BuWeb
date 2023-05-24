CREATE SCHEMA [logs];
GO


CREATE TABLE [dbo].[Articles] (
    [ArticleId] int NOT NULL IDENTITY,
    [Timestamp] rowversion NULL,
    [CreatedByUserId] int NOT NULL,
    [CreatedOnUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_Articles] PRIMARY KEY ([ArticleId])
);
GO


CREATE TABLE [logs].[EventLogs] (
    [EventLogId] int NOT NULL IDENTITY,
    [Timestamp] datetimeoffset NOT NULL,
    [SourceApplication] tinyint NOT NULL,
    [Level] tinyint NOT NULL,
    [MessageTemplate] nvarchar(max) NULL,
    [Message] nvarchar(max) NULL,
    [Exception] nvarchar(max) NULL,
    [LogEvent] nvarchar(max) NULL,
    [SourceContext] nvarchar(max) NULL,
    CONSTRAINT [PK_EventLogs] PRIMARY KEY ([EventLogId])
);
GO


CREATE TABLE [dbo].[WhitelistedIpAddresses] (
    [WhitelistIpAddressId] int NOT NULL IDENTITY,
    [Timestamp] rowversion NULL,
    [IpAddress] varchar(39) NOT NULL,
    CONSTRAINT [PK_WhitelistedIpAddresses] PRIMARY KEY ([WhitelistIpAddressId])
);
GO


CREATE TABLE [dbo].[AreaLayouts] (
    [AreaLayoutId] int NOT NULL IDENTITY,
    [Timestamp] rowversion NULL,
    [AreaId] int NOT NULL,
    [LayoutType] int NOT NULL,
    [LayoutName] varchar(500) NOT NULL,
    [LayoutPath] varchar(250) NOT NULL,
    [Status] tinyint NOT NULL,
    [CreatedByUserId] int NOT NULL,
    [CreatedOnUtc] datetime2 NOT NULL,
    [LastUpdatedByUserId] int NOT NULL,
    [LastUpdatedOnUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_AreaLayouts] PRIMARY KEY ([AreaLayoutId]),
    CONSTRAINT [UK_AreaLayouts_AreaId_AreaLayoutId] UNIQUE ([AreaId], [AreaLayoutId]),
    CONSTRAINT [UK_AreaLayouts_AreaLayoutId_LayoutName] UNIQUE ([AreaLayoutId], [LayoutName]),
    CONSTRAINT [UK_AreaLayouts_AreaLayoutId_LayoutPath] UNIQUE ([AreaLayoutId], [LayoutPath])
);
GO


CREATE TABLE [dbo].[Areas] (
    [AreaId] int NOT NULL IDENTITY,
    [Timestamp] rowversion NULL,
    [ParentAreaId] int NULL,
    [InstituteId] int NOT NULL,
    [InstituteLocationId] int NULL,
    [DepartmentId] int NULL,
    [AreaName] varchar(100) NOT NULL,
    [AreaPath] varchar(250) NOT NULL,
    [ContactOffice] varchar(500) NOT NULL,
    [Status] tinyint NOT NULL,
    [CreatedByUserId] int NOT NULL,
    [CreatedOnUtc] datetime2 NOT NULL,
    [LastUpdatedByUserId] int NOT NULL,
    [LastUpdatedOnUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_Areas] PRIMARY KEY ([AreaId]),
    CONSTRAINT [UK_Areas_AreaPath] UNIQUE ([AreaPath]),
    CONSTRAINT [FK_Areas_Areas_ParentAreaId] FOREIGN KEY ([ParentAreaId]) REFERENCES [dbo].[Areas] ([AreaId])
);
GO


CREATE TABLE [dbo].[ArticleDetails] (
    [ArticleDetailId] int NOT NULL IDENTITY,
    [Timestamp] rowversion NULL,
    [ArticleId] int NOT NULL,
    [PreviousArticleDetailId] int NULL,
    [AreaId] int NOT NULL,
    [AreaLayoutId] int NOT NULL,
    [ArticleUniqueId] nvarchar(max) NOT NULL,
    [ArticleName] nvarchar(max) NOT NULL,
    [ArticleType] int NOT NULL,
    [DefaultArticlePageId] int NULL,
    [CarouselArticleMediaId] int NULL,
    [SmallCarouselArticleMediaId] int NULL,
    [EventStartDate] datetime2 NULL,
    [EventEndDate] datetime2 NULL,
    [ArticleShortDescription] nvarchar(max) NULL,
    [CreatedByUserId] int NOT NULL,
    [CreatedOnUtc] datetime2 NOT NULL,
    [LastUpdatedByUserId] int NOT NULL,
    [LastUpdatedOnUtc] datetime2 NOT NULL,
    [PublishedByUserId] int NULL,
    [PublishedOnUtc] datetime2 NOT NULL,
    [ExpiryDateUtc] datetime2 NULL,
    [Status] tinyint NOT NULL,
    [Remarks] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_ArticleDetails] PRIMARY KEY ([ArticleDetailId]),
    CONSTRAINT [UK_ArticleDetails_ArticleId_ArticleDetailId] UNIQUE ([ArticleId], [ArticleDetailId]),
    CONSTRAINT [FK_ArticleDetails_AreaLayouts_AreaId_AreaLayoutId] FOREIGN KEY ([AreaId], [AreaLayoutId]) REFERENCES [dbo].[AreaLayouts] ([AreaId], [AreaLayoutId]),
    CONSTRAINT [FK_ArticleDetails_Areas_AreaId] FOREIGN KEY ([AreaId]) REFERENCES [dbo].[Areas] ([AreaId]),
    CONSTRAINT [FK_ArticleDetails_ArticleDetails_ArticleId_PreviousArticleDetailId] FOREIGN KEY ([ArticleId], [PreviousArticleDetailId]) REFERENCES [dbo].[ArticleDetails] ([ArticleId], [ArticleDetailId]),
    CONSTRAINT [FK_ArticleDetails_Articles_ArticleId] FOREIGN KEY ([ArticleId]) REFERENCES [dbo].[Articles] ([ArticleId])
);
GO


CREATE TABLE [dbo].[ArticleMedias] (
    [ArticleMediaId] int NOT NULL IDENTITY,
    [Timestamp] rowversion NULL,
    [ArticleDetailId] int NOT NULL,
    [ArticleMediaGuid] uniqueidentifier NOT NULL,
    [MediaUniqueId] nvarchar(100) NOT NULL,
    [MediaContentType] varchar(100) NOT NULL,
    [MediaContentDisposition] varchar(100) NULL,
    [MediaFileName] nvarchar(200) NOT NULL,
    [CreatedByUserId] int NOT NULL,
    [CreatedOnUtc] datetime2 NOT NULL,
    [LastUpdatedByUserId] int NOT NULL,
    [LastUpdatedOnUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_ArticleMedias] PRIMARY KEY ([ArticleMediaId]),
    CONSTRAINT [UK_ArticleMedias_ArticleDetailId_ArticleMediaId] UNIQUE ([ArticleDetailId], [ArticleMediaId]),
    CONSTRAINT [FK_ArticleMedias_ArticleDetails_ArticleDetailId] FOREIGN KEY ([ArticleDetailId]) REFERENCES [dbo].[ArticleDetails] ([ArticleDetailId])
);
GO


CREATE TABLE [dbo].[ArticlePages] (
    [ArticlePageId] int NOT NULL IDENTITY,
    [Timestamp] rowversion NULL,
    [ArticleDetailId] int NOT NULL,
    [ParentArticlePageId] int NULL,
    [PageUniqueId] nvarchar(max) NOT NULL,
    [MenuText] nvarchar(max) NULL,
    [PageSequence] int NOT NULL,
    [PageTitle] nvarchar(max) NOT NULL,
    [PageBody] nvarchar(max) NOT NULL,
    [PageBodyTextFormat] tinyint NOT NULL,
    [CreatedByUserId] int NOT NULL,
    [CreatedOnUtc] datetime2 NOT NULL,
    [LastUpdatedByUserId] int NOT NULL,
    [LastUpdatedOnUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_ArticlePages] PRIMARY KEY ([ArticlePageId]),
    CONSTRAINT [UK_ArticlePages_ArticleDetailId_ArticlePageId] UNIQUE ([ArticleDetailId], [ArticlePageId]),
    CONSTRAINT [FK_ArticlePages_ArticleDetails_ArticleDetailId] FOREIGN KEY ([ArticleDetailId]) REFERENCES [dbo].[ArticleDetails] ([ArticleDetailId]),
    CONSTRAINT [FK_ArticlePages_ArticlePages_ArticleDetailId_ParentArticlePageId] FOREIGN KEY ([ArticleDetailId], [ParentArticlePageId]) REFERENCES [dbo].[ArticlePages] ([ArticleDetailId], [ArticlePageId])
);
GO


CREATE TABLE [dbo].[Departments] (
    [DepartmentId] int NOT NULL IDENTITY,
    [Timestamp] rowversion NULL,
    [InstituteId] int NOT NULL,
    [InstituteLocationId] int NULL,
    [DepartmentName] varchar(500) NOT NULL,
    [DepartmentShortName] varchar(200) NOT NULL,
    [DepartmentAlias] varchar(50) NOT NULL,
    [Status] tinyint NOT NULL,
    [CreatedByUserId] int NOT NULL,
    [CreatedOnUtc] datetime2 NOT NULL,
    [LastUpdatedByUserId] int NOT NULL,
    [LastUpdatedOnUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_Departments] PRIMARY KEY ([DepartmentId]),
    CONSTRAINT [UK_Departments_InstituteId_DepartmentAlias] UNIQUE ([InstituteId], [DepartmentAlias]),
    CONSTRAINT [UK_Departments_InstituteId_DepartmentId] UNIQUE ([InstituteId], [DepartmentId]),
    CONSTRAINT [UK_Departments_InstituteId_DepartmentName] UNIQUE ([InstituteId], [DepartmentName]),
    CONSTRAINT [UK_Departments_InstituteId_DepartmentShortName] UNIQUE ([InstituteId], [DepartmentShortName])
);
GO


CREATE TABLE [dbo].[Users] (
    [UserId] int NOT NULL IDENTITY,
    [Timestamp] rowversion NULL,
    [Email] varchar(200) NOT NULL,
    [Name] varchar(200) NOT NULL,
    [DepartmentId] int NOT NULL,
    [MobileNo] varchar(20) NULL,
    [PhoneNo] varchar(20) NULL,
    [Status] tinyint NOT NULL,
    [CreatedByUserId] int NOT NULL,
    [CreatedOnUtc] datetime2 NOT NULL,
    [LastUpdatedByUserId] int NOT NULL,
    [LastUpdatedOnUtc] datetime2 NOT NULL,
    [ExpiryDateUtc] datetime2 NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([UserId]),
    CONSTRAINT [UK_Users_Email] UNIQUE ([Email]),
    CONSTRAINT [FK_Users_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [dbo].[Departments] ([DepartmentId]),
    CONSTRAINT [FK_Users_Users_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [FK_Users_Users_LastUpdatedByUserId] FOREIGN KEY ([LastUpdatedByUserId]) REFERENCES [dbo].[Users] ([UserId])
);
GO


CREATE TABLE [dbo].[Institutes] (
    [InstituteId] int NOT NULL,
    [Timestamp] rowversion NULL,
    [InstituteName] varchar(500) NOT NULL,
    [InstituteShortName] varchar(200) NOT NULL,
    [InstituteAlias] varchar(50) NOT NULL,
    [Status] tinyint NOT NULL,
    [CreatedByUserId] int NOT NULL,
    [CreatedOnUtc] datetime2 NOT NULL,
    [LastUpdatedByUserId] int NOT NULL,
    [LastUpdatedOnUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_Institutes] PRIMARY KEY ([InstituteId]),
    CONSTRAINT [UK_Institutes_InstituteAlias] UNIQUE ([InstituteAlias]),
    CONSTRAINT [UK_Institutes_InstituteName] UNIQUE ([InstituteName]),
    CONSTRAINT [UK_Institutes_InstituteShortName] UNIQUE ([InstituteShortName]),
    CONSTRAINT [FK_Institutes_Users_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [FK_Institutes_Users_LastUpdatedByUserId] FOREIGN KEY ([LastUpdatedByUserId]) REFERENCES [dbo].[Users] ([UserId])
);
GO


CREATE TABLE [dbo].[UserAreas] (
    [UserAreaId] int NOT NULL IDENTITY,
    [Timestamp] rowversion NULL,
    [UserId] int NOT NULL,
    [AreaId] int NOT NULL,
    [Role] tinyint NOT NULL,
    [InheritToAllChildren] bit NOT NULL,
    [Status] tinyint NOT NULL,
    [CreatedByUserId] int NOT NULL,
    [CreatedOnUtc] datetime2 NOT NULL,
    [LastUpdatedByUserId] int NOT NULL,
    [LastUpdatedOnUtc] datetime2 NOT NULL,
    [ExpiryDateUtc] datetime2 NULL,
    CONSTRAINT [PK_UserAreas] PRIMARY KEY ([UserAreaId]),
    CONSTRAINT [UK_UserAreas_UserId_AreaId] UNIQUE ([UserId], [AreaId]),
    CONSTRAINT [FK_UserAreas_Areas_AreaId] FOREIGN KEY ([AreaId]) REFERENCES [dbo].[Areas] ([AreaId]),
    CONSTRAINT [FK_UserAreas_Users_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [FK_UserAreas_Users_LastUpdatedByUserId] FOREIGN KEY ([LastUpdatedByUserId]) REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [FK_UserAreas_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
);
GO


CREATE TABLE [dbo].[UserRoles] (
    [UserRoleId] int NOT NULL IDENTITY,
    [Timestamp] rowversion NULL,
    [UserId] int NOT NULL,
    [Role] tinyint NOT NULL,
    [Status] tinyint NOT NULL,
    [CreatedByUserId] int NOT NULL,
    [CreatedOnUtc] datetime2 NOT NULL,
    [LastUpdatedByUserId] int NOT NULL,
    [LastUpdatedOnUtc] datetime2 NOT NULL,
    [ExpiryDateUtc] datetime2 NULL,
    CONSTRAINT [PK_UserRoles] PRIMARY KEY ([UserRoleId]),
    CONSTRAINT [UK_UserRoles_UserId_Role] UNIQUE ([UserId], [Role]),
    CONSTRAINT [FK_UserRoles_Users_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [FK_UserRoles_Users_LastUpdatedByUserId] FOREIGN KEY ([LastUpdatedByUserId]) REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [FK_UserRoles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
);
GO


CREATE TABLE [dbo].[YoutubeVideos] (
    [YoutubeVideoId] int NOT NULL IDENTITY,
    [Timestamp] rowversion NULL,
    [AreaId] int NOT NULL,
    [YoutubeUrl] nvarchar(450) COLLATE SQL_Latin1_General_CP1_CS_AS NOT NULL,
    [Status] tinyint NOT NULL,
    [Remarks] nvarchar(max) NOT NULL,
    [CreatedByUserId] int NOT NULL,
    [CreatedOnUtc] datetime2 NOT NULL,
    [LastUpdatedByUserId] int NOT NULL,
    [LastUpdatedOnUtc] datetime2 NOT NULL,
    [PublishedByUserId] int NULL,
    [PublishedOnUtc] datetime2 NOT NULL,
    [ExpiryDateUtc] datetime2 NULL,
    CONSTRAINT [PK_YoutubeVideos] PRIMARY KEY ([YoutubeVideoId]),
    CONSTRAINT [UK_YoutubeVideos_AreaId_YoutubeUrl] UNIQUE ([AreaId], [YoutubeUrl]),
    CONSTRAINT [FK_YoutubeVideos_Areas_AreaId] FOREIGN KEY ([AreaId]) REFERENCES [dbo].[Areas] ([AreaId]),
    CONSTRAINT [FK_YoutubeVideos_Users_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [FK_YoutubeVideos_Users_LastUpdatedByUserId] FOREIGN KEY ([LastUpdatedByUserId]) REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [FK_YoutubeVideos_Users_PublishedByUserId] FOREIGN KEY ([PublishedByUserId]) REFERENCES [dbo].[Users] ([UserId])
);
GO


CREATE TABLE [dbo].[InstituteLocations] (
    [InstituteLocationId] int NOT NULL IDENTITY,
    [Timestamp] rowversion NULL,
    [InstituteId] int NOT NULL,
    [LocationName] varchar(500) NOT NULL,
    [LocationShortName] varchar(200) NOT NULL,
    [LocationAlias] varchar(50) NOT NULL,
    [Status] tinyint NOT NULL,
    [CreatedByUserId] int NOT NULL,
    [CreatedOnUtc] datetime2 NOT NULL,
    [LastUpdatedByUserId] int NOT NULL,
    [LastUpdatedOnUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_InstituteLocations] PRIMARY KEY ([InstituteLocationId]),
    CONSTRAINT [UK_InstituteLocations_InstituteId_InstituteLocationId] UNIQUE ([InstituteId], [InstituteLocationId]),
    CONSTRAINT [UK_InstituteLocations_InstituteId_LocationAlias] UNIQUE ([InstituteId], [LocationAlias]),
    CONSTRAINT [UK_InstituteLocations_InstituteId_LocationName] UNIQUE ([InstituteId], [LocationName]),
    CONSTRAINT [UK_InstituteLocations_InstituteId_LocationShortName] UNIQUE ([InstituteId], [LocationShortName]),
    CONSTRAINT [FK_InstituteLocations_Institutes_InstituteId] FOREIGN KEY ([InstituteId]) REFERENCES [dbo].[Institutes] ([InstituteId]),
    CONSTRAINT [FK_InstituteLocations_Users_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [FK_InstituteLocations_Users_LastUpdatedByUserId] FOREIGN KEY ([LastUpdatedByUserId]) REFERENCES [dbo].[Users] ([UserId])
);
GO


CREATE INDEX [IX_AreaLayouts_CreatedByUserId] ON [dbo].[AreaLayouts] ([CreatedByUserId]);
GO


CREATE INDEX [IX_AreaLayouts_LastUpdatedByUserId] ON [dbo].[AreaLayouts] ([LastUpdatedByUserId]);
GO


CREATE INDEX [IX_Areas_CreatedByUserId] ON [dbo].[Areas] ([CreatedByUserId]);
GO


CREATE INDEX [IX_Areas_InstituteId_DepartmentId] ON [dbo].[Areas] ([InstituteId], [DepartmentId]);
GO


CREATE INDEX [IX_Areas_InstituteId_InstituteLocationId] ON [dbo].[Areas] ([InstituteId], [InstituteLocationId]);
GO


CREATE INDEX [IX_Areas_LastUpdatedByUserId] ON [dbo].[Areas] ([LastUpdatedByUserId]);
GO


CREATE INDEX [IX_Areas_ParentAreaId] ON [dbo].[Areas] ([ParentAreaId]);
GO


CREATE INDEX [IX_ArticleDetails_AreaId_AreaLayoutId] ON [dbo].[ArticleDetails] ([AreaId], [AreaLayoutId]);
GO


CREATE INDEX [IX_ArticleDetails_ArticleDetailId_CarouselArticleMediaId] ON [dbo].[ArticleDetails] ([ArticleDetailId], [CarouselArticleMediaId]);
GO


CREATE INDEX [IX_ArticleDetails_ArticleDetailId_DefaultArticlePageId] ON [dbo].[ArticleDetails] ([ArticleDetailId], [DefaultArticlePageId]);
GO


CREATE INDEX [IX_ArticleDetails_ArticleDetailId_SmallCarouselArticleMediaId] ON [dbo].[ArticleDetails] ([ArticleDetailId], [SmallCarouselArticleMediaId]);
GO


CREATE INDEX [IX_ArticleDetails_ArticleId_PreviousArticleDetailId] ON [dbo].[ArticleDetails] ([ArticleId], [PreviousArticleDetailId]);
GO


CREATE INDEX [IX_ArticleDetails_CreatedByUserId] ON [dbo].[ArticleDetails] ([CreatedByUserId]);
GO


CREATE INDEX [IX_ArticleDetails_LastUpdatedByUserId] ON [dbo].[ArticleDetails] ([LastUpdatedByUserId]);
GO


CREATE INDEX [IX_ArticleDetails_PublishedByUserId] ON [dbo].[ArticleDetails] ([PublishedByUserId]);
GO


CREATE INDEX [IX_ArticleMedias_CreatedByUserId] ON [dbo].[ArticleMedias] ([CreatedByUserId]);
GO


CREATE INDEX [IX_ArticleMedias_LastUpdatedByUserId] ON [dbo].[ArticleMedias] ([LastUpdatedByUserId]);
GO


CREATE INDEX [IX_ArticlePages_ArticleDetailId_ParentArticlePageId] ON [dbo].[ArticlePages] ([ArticleDetailId], [ParentArticlePageId]);
GO


CREATE INDEX [IX_ArticlePages_CreatedByUserId] ON [dbo].[ArticlePages] ([CreatedByUserId]);
GO


CREATE INDEX [IX_ArticlePages_LastUpdatedByUserId] ON [dbo].[ArticlePages] ([LastUpdatedByUserId]);
GO


CREATE INDEX [IX_Departments_CreatedByUserId] ON [dbo].[Departments] ([CreatedByUserId]);
GO


CREATE INDEX [IX_Departments_InstituteId_InstituteLocationId] ON [dbo].[Departments] ([InstituteId], [InstituteLocationId]);
GO


CREATE INDEX [IX_Departments_LastUpdatedByUserId] ON [dbo].[Departments] ([LastUpdatedByUserId]);
GO


CREATE INDEX [IX_InstituteLocations_CreatedByUserId] ON [dbo].[InstituteLocations] ([CreatedByUserId]);
GO


CREATE INDEX [IX_InstituteLocations_LastUpdatedByUserId] ON [dbo].[InstituteLocations] ([LastUpdatedByUserId]);
GO


CREATE INDEX [IX_Institutes_CreatedByUserId] ON [dbo].[Institutes] ([CreatedByUserId]);
GO


CREATE INDEX [IX_Institutes_LastUpdatedByUserId] ON [dbo].[Institutes] ([LastUpdatedByUserId]);
GO


CREATE INDEX [IX_UserAreas_AreaId] ON [dbo].[UserAreas] ([AreaId]);
GO


CREATE INDEX [IX_UserAreas_CreatedByUserId] ON [dbo].[UserAreas] ([CreatedByUserId]);
GO


CREATE INDEX [IX_UserAreas_LastUpdatedByUserId] ON [dbo].[UserAreas] ([LastUpdatedByUserId]);
GO


CREATE INDEX [IX_UserRoles_CreatedByUserId] ON [dbo].[UserRoles] ([CreatedByUserId]);
GO


CREATE INDEX [IX_UserRoles_LastUpdatedByUserId] ON [dbo].[UserRoles] ([LastUpdatedByUserId]);
GO


CREATE INDEX [IX_Users_CreatedByUserId] ON [dbo].[Users] ([CreatedByUserId]);
GO


CREATE INDEX [IX_Users_DepartmentId] ON [dbo].[Users] ([DepartmentId]);
GO


CREATE INDEX [IX_Users_LastUpdatedByUserId] ON [dbo].[Users] ([LastUpdatedByUserId]);
GO


CREATE INDEX [IX_YoutubeVideos_CreatedByUserId] ON [dbo].[YoutubeVideos] ([CreatedByUserId]);
GO


CREATE INDEX [IX_YoutubeVideos_LastUpdatedByUserId] ON [dbo].[YoutubeVideos] ([LastUpdatedByUserId]);
GO


CREATE INDEX [IX_YoutubeVideos_PublishedByUserId] ON [dbo].[YoutubeVideos] ([PublishedByUserId]);
GO


ALTER TABLE [dbo].[AreaLayouts] ADD CONSTRAINT [FK_AreaLayouts_Areas_AreaId] FOREIGN KEY ([AreaId]) REFERENCES [dbo].[Areas] ([AreaId]);
GO


ALTER TABLE [dbo].[AreaLayouts] ADD CONSTRAINT [FK_AreaLayouts_Users_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([UserId]);
GO


ALTER TABLE [dbo].[AreaLayouts] ADD CONSTRAINT [FK_AreaLayouts_Users_LastUpdatedByUserId] FOREIGN KEY ([LastUpdatedByUserId]) REFERENCES [dbo].[Users] ([UserId]);
GO


ALTER TABLE [dbo].[Areas] ADD CONSTRAINT [FK_Areas_Departments_InstituteId_DepartmentId] FOREIGN KEY ([InstituteId], [DepartmentId]) REFERENCES [dbo].[Departments] ([InstituteId], [DepartmentId]);
GO


ALTER TABLE [dbo].[Areas] ADD CONSTRAINT [FK_Areas_InstituteLocations_InstituteId_InstituteLocationId] FOREIGN KEY ([InstituteId], [InstituteLocationId]) REFERENCES [dbo].[InstituteLocations] ([InstituteId], [InstituteLocationId]);
GO


ALTER TABLE [dbo].[Areas] ADD CONSTRAINT [FK_Areas_Institutes_InstituteId] FOREIGN KEY ([InstituteId]) REFERENCES [dbo].[Institutes] ([InstituteId]);
GO


ALTER TABLE [dbo].[Areas] ADD CONSTRAINT [FK_Areas_Users_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([UserId]);
GO


ALTER TABLE [dbo].[Areas] ADD CONSTRAINT [FK_Areas_Users_LastUpdatedByUserId] FOREIGN KEY ([LastUpdatedByUserId]) REFERENCES [dbo].[Users] ([UserId]);
GO


ALTER TABLE [dbo].[ArticleDetails] ADD CONSTRAINT [FK_ArticleDetails_ArticleMedias_ArticleDetailId_CarouselArticleMediaId] FOREIGN KEY ([ArticleDetailId], [CarouselArticleMediaId]) REFERENCES [dbo].[ArticleMedias] ([ArticleDetailId], [ArticleMediaId]);
GO


ALTER TABLE [dbo].[ArticleDetails] ADD CONSTRAINT [FK_ArticleDetails_ArticleMedias_ArticleDetailId_SmallCarouselArticleMediaId] FOREIGN KEY ([ArticleDetailId], [SmallCarouselArticleMediaId]) REFERENCES [dbo].[ArticleMedias] ([ArticleDetailId], [ArticleMediaId]);
GO


ALTER TABLE [dbo].[ArticleDetails] ADD CONSTRAINT [FK_ArticleDetails_ArticlePages_ArticleDetailId_DefaultArticlePageId] FOREIGN KEY ([ArticleDetailId], [DefaultArticlePageId]) REFERENCES [dbo].[ArticlePages] ([ArticleDetailId], [ArticlePageId]);
GO


ALTER TABLE [dbo].[ArticleDetails] ADD CONSTRAINT [FK_ArticleDetails_Users_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([UserId]);
GO


ALTER TABLE [dbo].[ArticleDetails] ADD CONSTRAINT [FK_ArticleDetails_Users_LastUpdatedByUserId] FOREIGN KEY ([LastUpdatedByUserId]) REFERENCES [dbo].[Users] ([UserId]);
GO


ALTER TABLE [dbo].[ArticleDetails] ADD CONSTRAINT [FK_ArticleDetails_Users_PublishedByUserId] FOREIGN KEY ([PublishedByUserId]) REFERENCES [dbo].[Users] ([UserId]);
GO


ALTER TABLE [dbo].[ArticleMedias] ADD CONSTRAINT [FK_ArticleMedias_Users_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([UserId]);
GO


ALTER TABLE [dbo].[ArticleMedias] ADD CONSTRAINT [FK_ArticleMedias_Users_LastUpdatedByUserId] FOREIGN KEY ([LastUpdatedByUserId]) REFERENCES [dbo].[Users] ([UserId]);
GO


ALTER TABLE [dbo].[ArticlePages] ADD CONSTRAINT [FK_ArticlePages_Users_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([UserId]);
GO


ALTER TABLE [dbo].[ArticlePages] ADD CONSTRAINT [FK_ArticlePages_Users_LastUpdatedByUserId] FOREIGN KEY ([LastUpdatedByUserId]) REFERENCES [dbo].[Users] ([UserId]);
GO


ALTER TABLE [dbo].[Departments] ADD CONSTRAINT [FK_Departments_InstituteLocations_InstituteId_InstituteLocationId] FOREIGN KEY ([InstituteId], [InstituteLocationId]) REFERENCES [dbo].[InstituteLocations] ([InstituteId], [InstituteLocationId]);
GO


ALTER TABLE [dbo].[Departments] ADD CONSTRAINT [FK_Departments_Institutes_InstituteId] FOREIGN KEY ([InstituteId]) REFERENCES [dbo].[Institutes] ([InstituteId]);
GO


ALTER TABLE [dbo].[Departments] ADD CONSTRAINT [FK_Departments_Users_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([UserId]);
GO


ALTER TABLE [dbo].[Departments] ADD CONSTRAINT [FK_Departments_Users_LastUpdatedByUserId] FOREIGN KEY ([LastUpdatedByUserId]) REFERENCES [dbo].[Users] ([UserId]);
GO


