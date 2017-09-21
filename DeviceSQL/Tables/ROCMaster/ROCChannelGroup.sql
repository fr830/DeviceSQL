﻿CREATE TABLE [ROCMaster].[ROCChannelGroup] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (255) NOT NULL,
    [Timeout]     INT           NOT NULL,
    [HostAddress] TINYINT       NOT NULL,
    [HostGroup]   TINYINT       NOT NULL,
    [RowVersion]  ROWVERSION    NULL,
    CONSTRAINT [PK_ROCChannelGroup] PRIMARY KEY CLUSTERED ([Id] ASC)
);
