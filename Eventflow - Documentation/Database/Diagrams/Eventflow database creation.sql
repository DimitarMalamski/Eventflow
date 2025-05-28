-- Database Selection
USE [dbi546327_eventflow]
GO

-- Role Table
CREATE TABLE [Role] (
	[Id] INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(20) NOT NULL
);

-- User Table
CREATE TABLE [User] (
	[Id] INT PRIMARY KEY IDENTITY,
	[Username] NVARCHAR(50) UNIQUE NOT NULL,
	[PasswordHash] NVARCHAR(255) NOT NULL,
	[Salt] NVARCHAR(255) NOT NULL,
	[Firstname] NVARCHAR(50) NOT NULL,
	[Lastname] NVARCHAR(50),
	[Email] NVARCHAR(256) UNIQUE NOT NULL,
	[IsBanned] BIT DEFAULT 0,
	[IsDeleted] BIT DEFAULT 0,
	[RoleId] INT FOREIGN KEY REFERENCES [Role](Id) NOT NULL,
);

-- Category Table
CREATE TABLE [Category] (
	[Id] INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(50) NOT NULL
);

-- Personal Event Table
CREATE TABLE [PersonalEvent] (
	[Id] INT PRIMARY KEY IDENTITY,
	[Title] NVARCHAR(50) NOT NULL,
	[Description] NVARCHAR(256),
	[IsCompleted] BIT DEFAULT 0,
	[Date] DATETIME NOT NULL,
	[IsDeleted] BIT DEFAULT 0,
	[IsGlobal] BIT DEFAULT 0,
	[CategoryId] INT FOREIGN KEY REFERENCES [Category](Id),
	[UserId] INT FOREIGN KEY REFERENCES [User](Id) NOT NULL
);

-- Personal Event Reminder Table
CREATE TABLE [PersonalEventReminder] (
	[Id] INT PRIMARY KEY IDENTITY,
	[Title] NVARCHAR(50) NOT NULL,
	[Description] NVARCHAR(256),
	[Date] DATETIME NOT NULL,
	[ReadAt] DATETIME NOT NULL,
	[IsRead] BIT DEFAULT 0,
	[IsLiked] BIT DEFAULT 0,
	[PersonalEventId] INT FOREIGN KEY REFERENCES [PersonalEvent](Id) NOT NULL,
	[UserId] INT FOREIGN KEY REFERENCES [User](Id) NOT NULL
);

-- Continent Table
CREATE TABLE [Continent] (
	[Id] INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(50) NOT NULL
);

-- Country Table
CREATE TABLE [Country] (
	[Id] INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(50) NOT NULL,
	[FlagUrl] NVARCHAR(255),
	[ISOCode] NVARCHAR(3),
	[ContinentId] INT FOREIGN KEY REFERENCES [Continent](Id) NOT NULL
);

-- Bookmarked Events Table
CREATE TABLE [Bookmarked] (
	[Id] INT PRIMARY KEY IDENTITY,
	[Date] DATETIME NOT NULL,
	[UserId] INT FOREIGN KEY REFERENCES [User](Id) NOT NULL,
	[CountryId] INT FOREIGN KEY REFERENCES [Country](Id) NOT NULL 
);

-- National Event Table
CREATE TABLE [NationalEvent] (
	[Id] INT PRIMARY KEY IDENTITY,
	[Title] NVARCHAR(255),
	[Description] NVARCHAR(MAX),
	[Date] DATETIME NOT NULL,
	[CountryId] INT FOREIGN KEY REFERENCES [Country](Id) NOT NULL
);

-- National Event Reminder Table
CREATE TABLE [NationalEventReminder] (
	[Id] INT PRIMARY KEY IDENTITY,
	[Title] NVARCHAR(50) NOT NULL,
	[Description] NVARCHAR(256),
	[Date] DATETIME NOT NULL,
	[IsRead] BIT DEFAULT 0,
	[IsLiked] BIT DEFAULT 0,
	[ReadAt] DATETIME,
	[NationalEventId] INT FOREIGN KEY REFERENCES [NationalEvent](Id) NOT NULL
);

-- Status Table
CREATE TABLE [Status] (
	[Id] INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(50) NOT NULL UNIQUE
);

-- Invite Table
CREATE TABLE [Invite] (
	[Id] INT PRIMARY KEY IDENTITY,
	[PersonalEventId] INT FOREIGN KEY REFERENCES [PersonalEvent](Id) NOT NULL,
	[InvitedUserId] INT FOREIGN KEY REFERENCES [User](Id) NOT NULL,
	[StatusId] INT FOREIGN KEY REFERENCES [Status](Id) NOT NULL,
	[CreatedAt] DATETIME DEFAULT GETDATE() NOT NULL,
	CONSTRAINT UQ_Invite UNIQUE ([PersonalEventId], [InvitedUserId])
);

INSERT INTO Role (Name) VALUES ('Admin');
INSERT INTO Role (Name) VALUES ('User');