USE [CC-TechTest-SQLServer]
GO

ALTER TABLE [dbo].[RegisteredMeters] DROP CONSTRAINT [CK_POSTCODE_FORMAT]
GO

ALTER TABLE [dbo].[RegisteredMeters] DROP CONSTRAINT [CK_MPAN_LENGTH]
GO

ALTER TABLE [dbo].[RegisteredMeters] DROP CONSTRAINT [CK_MPAN_FULL_LENGTH]
GO

ALTER TABLE [dbo].[RegisteredMeters] DROP CONSTRAINT [CK_DATE_IS_PAST]
GO

/****** Object:  Table [dbo].[RegisteredMeters]    Script Date: 16/07/2025 21:39:27 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RegisteredMeters]') AND type in (N'U'))
DROP TABLE [dbo].[RegisteredMeters]
GO

/****** Object:  Table [dbo].[RegisteredMeters]    Script Date: 16/07/2025 21:39:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[RegisteredMeters](
	[MPAN] [numeric](13, 0) NOT NULL,
	[MeterSerial] [varchar](10) NOT NULL,
	[DateOfInstallation] [date] NOT NULL,
	[AddressLine1] [varchar](40) NULL,
	[PostCode] [varchar](10) NULL,
 CONSTRAINT [KEY_REGISTERED_METERS] UNIQUE NONCLUSTERED 
(
	[MPAN] ASC,
	[DateOfInstallation] ASC,
	[MeterSerial] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[RegisteredMeters]  WITH CHECK ADD  CONSTRAINT [CK_DATE_IS_PAST] CHECK  (([DateOfInstallation]<CONVERT([date],getdate())))
GO

ALTER TABLE [dbo].[RegisteredMeters] CHECK CONSTRAINT [CK_DATE_IS_PAST]
GO

ALTER TABLE [dbo].[RegisteredMeters]  WITH CHECK ADD  CONSTRAINT [CK_MPAN_FULL_LENGTH] CHECK  ((len(CONVERT([varchar],[MPAN]))=(13)))
GO

ALTER TABLE [dbo].[RegisteredMeters] CHECK CONSTRAINT [CK_MPAN_FULL_LENGTH]
GO

ALTER TABLE [dbo].[RegisteredMeters]  WITH CHECK ADD  CONSTRAINT [CK_SERIAL_LENGTH] CHECK  ((len([MeterSerial])>=(1) AND len([MeterSerial])<=(10)))
GO

ALTER TABLE [dbo].[RegisteredMeters] CHECK CONSTRAINT [CK_SERIAL_LENGTH]
GO

ALTER TABLE [dbo].[RegisteredMeters]  WITH CHECK ADD  CONSTRAINT [CK_POSTCODE_FORMAT] CHECK  (([PostCode] IS NULL OR [PostCode] like '[A-Z][A-Z][0-9] [0-9][A-Z][A-Z]'))
GO

ALTER TABLE [dbo].[RegisteredMeters] CHECK CONSTRAINT [CK_POSTCODE_FORMAT]
GO


